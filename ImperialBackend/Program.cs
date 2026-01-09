// Program.cs
// ASP.NET Core Web API + Authentik (OIDC) + Cookie auth
// Outage-friendly:
//  - DB can be down without crashing the host
//  - BackgroundService exceptions won't take down the API
//  - SQL exceptions from controllers map to 503
//  - GuildMemberSyncService registered as a proper HostedService (no singleton hack)
//  - HttpClientFactory enabled

using System.Security.Claims;
using System.Threading.RateLimiting;
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

/* ============================
 * Host Options (Outage-friendly)
 * ============================ */

// Do NOT crash the entire web host if a BackgroundService throws.
// (Workers should still handle outages internally, but this prevents API shutdown.)
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

/* ============================
 * Services
 * ============================ */

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContextFactory<ImperialDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            // Helps for transient failures (doesn't fix outages, but improves resilience).
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

// App services
builder.Services.AddScoped<IRaidsCompletedService, RaidsCompletedService>();
builder.Services.AddScoped<IGuildMemberService, GuildMemberService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMedalService, MedalService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IRaidService, RaidService>();

// Background services
builder.Services.AddHostedService<ImperialBackend.Messaging.RaidCompletedConsumer>();

// GuildMemberSyncService uses external APIs -> use IHttpClientFactory
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
    return "ip:" + (ip ?? "unknown"); // anonymous/unauthenticated users
}

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var key = GetRateLimitKey(ctx);
        var isAuthenticated = key.StartsWith("u:");
        var perMinute = isAuthenticated ? 200 : 100; // ~200 req/min auth, ~100 req/min anon

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

    // Strict for login/callback endpoints (per IP)
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

/* ============================
 * Logging
 * ============================ */

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

static string ToPublicUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url))
        return url;

    // Needs Secret
    return url.Replace("http://authentik-server:9000", "http://192.168.4.121:9000")
        .Replace("https://authentik-server:9000", "http://192.168.4.121:9000")
        .Replace("http://backend:5032", "http://192.168.4.121:5032")
        .Replace("https://backend:5032", "http://192.168.4.121:5032");
}

/* ============================
 * Authentik / OIDC config
 * ============================ */

var authentikAuthority =
    builder.Configuration["Authentik:Authority"]
    ?? throw new InvalidOperationException("Authentik:Authority not configured");

// Internal URL is for backchannel metadata fetch from inside Docker.
var authentikInternalUrl = builder.Configuration["Authentik:InternalUrl"] ?? authentikAuthority;

var clientId =
    builder.Configuration["Authentik:ClientId"]
    ?? throw new InvalidOperationException("Authentik:ClientId not configured");
var clientSecret =
    builder.Configuration["Authentik:ClientSecret"]
    ?? throw new InvalidOperationException("Authentik:ClientSecret not configured");

// IMPORTANT: Authority should be the server root, e.g. http://localhost:9000
// NOT /application/o/imperial-web/
authentikAuthority = authentikAuthority.TrimEnd('/');
authentikInternalUrl = authentikInternalUrl.TrimEnd('/');

// This is the callback URL Authentik must allow, and what the browser can resolve.
const string publicBackendBaseUrl = "http://192.168.4.121:5032";
var callbackUrl = $"{publicBackendBaseUrl}/api/auth/callback";
var signoutCallbackUrl = $"{publicBackendBaseUrl}/api/auth/signout-callback";

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

            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;

            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);

            // APIs should return 401 instead of redirecting to login
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        }
    )
    .AddOpenIdConnect(
        OpenIdConnectDefaults.AuthenticationScheme,
        options =>
        {
            // ✅ Explicitly tell OIDC to sign in using the cookie scheme
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

            // OIDC callback endpoints on THIS backend
            options.CallbackPath = "/api/auth/callback";
            options.SignedOutCallbackPath = "/api/auth/signout-callback";

            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.MapInboundClaims = false;

            // ✅ Backchannel metadata fetch uses internal Docker URL (safe + reliable)
            options.MetadataAddress = $"{authentikInternalUrl}/.well-known/openid-configuration";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
                RoleClaimType = "role",
                ValidIssuer = authentikAuthority,
            };

            // Temp cookies for OIDC correlation/nonce
            options.CorrelationCookie.Name = "imperial.oidc.correlation";
            options.CorrelationCookie.HttpOnly = true;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
            options.CorrelationCookie.Expiration = TimeSpan.FromMinutes(15);

            options.NonceCookie.Name = "imperial.oidc.nonce";
            options.NonceCookie.HttpOnly = true;
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.NonceCookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
            options.NonceCookie.Expiration = TimeSpan.FromMinutes(15);

            options.Events = new OpenIdConnectEvents
            {
                // LOGIN
                OnRedirectToIdentityProvider = context =>
                {
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.IssuerAddress))
                        context.ProtocolMessage.IssuerAddress = ToPublicUrl(
                            context.ProtocolMessage.IssuerAddress
                        );

                    if (!string.IsNullOrEmpty(context.ProtocolMessage.RedirectUri))
                        context.ProtocolMessage.RedirectUri = ToPublicUrl(
                            context.ProtocolMessage.RedirectUri
                        );

                    return Task.CompletedTask;
                },

                // LOGOUT
                OnRedirectToIdentityProviderForSignOut = context =>
                {
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

                // AFTER LOGIN CALLBACK
                OnTicketReceived = context =>
                {
                    var redirectUri = context.Properties?.RedirectUri;

                    if (!string.IsNullOrEmpty(redirectUri))
                        context.Properties!.RedirectUri = ToPublicUrl(redirectUri);

                    return Task.CompletedTask;
                },

                // AFTER LOGOUT CALLBACK
                OnSignedOutCallbackRedirect = context =>
                {
                    context.Response.Redirect("http://192.168.4.121:5173/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },

                // FAILURE
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("http://192.168.4.121:5173/login?error=auth_failed");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
            };
        }
    );

/* ============================
 * App + Middleware
 * ============================ */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Do NOT run migrations at startup in an outage simulation.
    // If you want migrations, gate behind a config flag and wrap in try/catch.
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

// Convert SQL failures from request pipeline into a clean 503 instead of crashing or 500 spam.
// This does NOT affect background services; those should handle retries internally.
app.Map(
    "/error",
    async (HttpContext ctx) =>
    {
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ctx.Response.HasStarted)
            return;

        ctx.Response.ContentType = "application/json";

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
