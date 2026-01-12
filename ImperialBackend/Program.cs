// Program.cs
// ASP.NET Core Web API + Authentik (OIDC) + Cookie auth

using System.Security.Claims;
using System.Threading.RateLimiting;
using ImperialBackend.Config;
using ImperialBackend.Models;
using ImperialBackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables();

static string Env(string name)
{
    var value = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException(
            $"{name} is not set (check .env / docker compose env)."
        );
    return value.TrimEnd('/');
}

var frontendUrl = Env("FRONTEND_URL");
var backendUrl = Env("BACKEND_URL");
var authentikUrl = Env("AUTHENTIK_URL");
var clientId = Env("CLIENT_ID");
var clientSecret = Env("CLIENT_SECRET");
var issuerPath = Env("AUTHENTIK_ISSUER_PATH");
var authentikInternalUrl = Env("AUTHENTIK_INTERNAL_URL");
var backendInternalUrl = Env("BACKEND_INTERNAL_URL");

Console.WriteLine($"[Auth Config] Frontend URL: {frontendUrl}");
Console.WriteLine($"[Auth Config] Backend URL: {backendUrl}");
Console.WriteLine($"[Auth Config] Authentik URL: {authentikUrl}");
Console.WriteLine($"[Auth Config] Issuer Path: {issuerPath}");
Console.WriteLine($"[Auth Config] Authentik Internal URL: {authentikInternalUrl}");

var cs = SecretReader.Get("ConnectionStrings__DefaultConnection", required: false);
if (!string.IsNullOrWhiteSpace(cs))
    builder.Configuration["ConnectionStrings:DefaultConnection"] = cs;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContextFactory<ImperialDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
        }
    )
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy =>
            policy.RequireAssertion(ctx =>
                ctx.User.Claims.Any(c => c.Type == "groups" && c.Value == "admins")
            )
    );
});

builder.Services.AddScoped<IRaidsCompletedService, RaidsCompletedService>();
builder.Services.AddScoped<IGuildMemberService, GuildMemberService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMedalService, MedalService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IRaidService, RaidService>();

builder.Services.AddHostedService<ImperialBackend.Messaging.RaidCompletedConsumer>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<GuildMemberSyncService>();

static string GetRateLimitKey(HttpContext ctx)
{
    if (ctx.User?.Identity?.IsAuthenticated == true)
    {
        var sub =
            ctx.User.FindFirst("sub")?.Value
            ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? ctx.User.Identity?.Name;

        return "u:" + (sub ?? "unknown");
    }

    var ip = ctx.Connection.RemoteIpAddress?.ToString();
    return "ip:" + (ip ?? "unknown");
}

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var key = GetRateLimitKey(ctx);
        var isAuthenticated = key.StartsWith("u:");
        var perMinute = isAuthenticated ? 200 : 100;

        return RateLimitPartition.GetTokenBucketLimiter(
            key,
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = perMinute,
                TokensPerPeriod = perMinute,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueLimit = 0,
            }
        );
    });

    options.AddPolicy(
        "auth",
        ctx =>
        {
            var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = "ip:" + ip;

            return RateLimitPartition.GetFixedWindowLimiter(
                key,
                _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }
            );
        }
    );

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.",
            token
        );
    };
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

string ToPublicUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url))
        return url;

    return url.Replace(authentikInternalUrl, authentikUrl).Replace(backendInternalUrl, backendUrl);
}

// CRITICAL: Use internal URL for metadata fetching, public URL for authority/issuer validation
var authentikAuthority = issuerPath.TrimEnd('/');
var authentikInternalIssuer = $"{authentikInternalUrl}/application/o/imperial-web";

var callbackUrl = $"{backendUrl.TrimEnd('/')}/api/auth/callback";
var signoutCallbackUrl = $"{backendUrl.TrimEnd('/')}/api/auth/signout-callback";
Console.WriteLine($"[Auth] Callback URL: {callbackUrl}");
Console.WriteLine($"[Auth] Signout Callback URL: {signoutCallbackUrl}");
Console.WriteLine($"[Auth] Authority (public): {authentikAuthority}");
Console.WriteLine(
    $"[Auth] Internal Metadata URL: {authentikInternalIssuer}/.well-known/openid-configuration"
);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.Cookie.Name = "imperial.auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);

            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    Console.WriteLine(
                        $"[Auth] API request to {context.Request.Path} - returning 401"
                    );
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                Console.WriteLine($"[Auth] Redirecting to login: {context.RedirectUri}");
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        }
    )
    .AddOpenIdConnect(
        OpenIdConnectDefaults.AuthenticationScheme,
        options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            options.Authority = authentikAuthority;
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;

            options.ResponseType = "code";
            options.ResponseMode = "query";

            options.SaveTokens = false;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            options.CallbackPath = "/api/auth/callback";
            options.SignedOutCallbackPath = "/api/auth/signout-callback";

            options.RequireHttpsMetadata = false;
            options.MapInboundClaims = false;

            // CRITICAL: Use internal URL for metadata fetching
            options.MetadataAddress = $"{authentikInternalIssuer}/.well-known/openid-configuration";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
                RoleClaimType = "role",
                ValidIssuer = authentikAuthority,
            };

            options.CorrelationCookie.Name = "imperial.oidc.correlation";
            options.CorrelationCookie.HttpOnly = true;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.CorrelationCookie.Expiration = TimeSpan.FromMinutes(15);

            options.NonceCookie.Name = "imperial.oidc.nonce";
            options.NonceCookie.HttpOnly = true;
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.NonceCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.NonceCookie.Expiration = TimeSpan.FromMinutes(15);

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context =>
                {
                    Console.WriteLine($"[Auth] OnRedirectToIdentityProvider");
                    Console.WriteLine(
                        $"[Auth] Original IssuerAddress: {context.ProtocolMessage.IssuerAddress}"
                    );
                    Console.WriteLine(
                        $"[Auth] Original RedirectUri: {context.ProtocolMessage.RedirectUri}"
                    );

                    if (!string.IsNullOrEmpty(context.ProtocolMessage.IssuerAddress))
                        context.ProtocolMessage.IssuerAddress = ToPublicUrl(
                            context.ProtocolMessage.IssuerAddress
                        );

                    if (!string.IsNullOrEmpty(context.ProtocolMessage.RedirectUri))
                        context.ProtocolMessage.RedirectUri = ToPublicUrl(
                            context.ProtocolMessage.RedirectUri
                        );

                    Console.WriteLine(
                        $"[Auth] Updated IssuerAddress: {context.ProtocolMessage.IssuerAddress}"
                    );
                    Console.WriteLine(
                        $"[Auth] Updated RedirectUri: {context.ProtocolMessage.RedirectUri}"
                    );

                    return Task.CompletedTask;
                },

                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    Console.WriteLine($"[Auth] OnRedirectToIdentityProviderForSignOut");

                    if (!string.IsNullOrEmpty(context.ProtocolMessage.IssuerAddress))
                        context.ProtocolMessage.IssuerAddress = ToPublicUrl(
                            context.ProtocolMessage.IssuerAddress
                        );

                    if (!string.IsNullOrEmpty(context.ProtocolMessage.PostLogoutRedirectUri))
                        context.ProtocolMessage.PostLogoutRedirectUri = ToPublicUrl(
                            context.ProtocolMessage.PostLogoutRedirectUri
                        );

                    return Task.CompletedTask;
                },

                OnTicketReceived = context =>
                {
                    Console.WriteLine($"[Auth] OnTicketReceived - Login successful");
                    var redirectUri = context.Properties?.RedirectUri;

                    if (!string.IsNullOrEmpty(redirectUri))
                        context.Properties!.RedirectUri = ToPublicUrl(redirectUri);

                    return Task.CompletedTask;
                },

                OnSignedOutCallbackRedirect = context =>
                {
                    Console.WriteLine(
                        $"[Auth] OnSignedOutCallbackRedirect - Redirecting to frontend"
                    );
                    context.Response.Redirect(frontendUrl);
                    context.HandleResponse();
                    return Task.CompletedTask;
                },

                OnRemoteFailure = context =>
                {
                    Console.WriteLine($"[Auth ERROR] OnRemoteFailure: {context.Failure?.Message}");
                    Console.WriteLine($"[Auth ERROR] Exception: {context.Failure}");
                    context.Response.Redirect($"{frontendUrl}/login?error=auth_failed");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
            };
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");
app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor
            | ForwardedHeaders.XForwardedProto
            | ForwardedHeaders.XForwardedHost,
    }
);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.Map(
    "/error",
    async (HttpContext ctx) =>
    {
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        Console.WriteLine($"[ERROR] Exception caught: {ex?.GetType().Name}");
        Console.WriteLine($"[ERROR] Message: {ex?.Message}");
        Console.WriteLine($"[ERROR] Stack: {ex?.StackTrace}");

        if (ctx.Response.HasStarted)
            return;

        ctx.Response.ContentType = "application/json";

        if (ex is KeyNotFoundException)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "NotFound", message = "Resource not found." }
            );
            return;
        }

        if (ex is SqlException)
        {
            ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await ctx.Response.WriteAsJsonAsync(
                new
                {
                    error = "DatabaseUnavailable",
                    message = "Service is currently unavailable. Please try again later.",
                }
            );
            return;
        }

        if (IsAuthentikOidcDown(ex))
        {
            Console.WriteLine($"[ERROR] Authentik OIDC is down - returning 503");
            ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await ctx.Response.WriteAsJsonAsync(
                new
                {
                    error = "AuthProviderUnavailable",
                    message = "Login is currently unavailable. Please try again later.",
                }
            );
            return;
        }

        if (ex is InvalidOperationException)
        {
            ctx.Response.StatusCode = StatusCodes.Status409Conflict;
            await ctx.Response.WriteAsJsonAsync(new { error = "Conflict", message = ex.Message });
            return;
        }

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(
            new { error = "UnhandledException", message = "An unexpected error occurred." }
        );
    }
);

static bool IsAuthentikOidcDown(Exception? ex)
{
    for (var cur = ex; cur != null; cur = cur.InnerException)
    {
        var msg = cur.Message ?? "";
        if (
            msg.Contains("IDX20803", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("IDX20804", StringComparison.OrdinalIgnoreCase)
        )
            return true;
    }
    return false;
}

app.MapControllers();
app.Run();
