// Program.cs
// ASP.NET Core Web API + Authentik (OIDC) + Cookie auth
// Flow:
//   1) Frontend sends user to: GET /api/auth/login?returnUrl=http://localhost:5173/somepage
//   2) Backend challenges with OIDC -> browser goes to Authentik
//   3) Authentik posts back to: POST http://localhost:5032/api/auth/callback
//   4) OIDC middleware validates, creates cookie, then redirects user to returnUrl
//   5) Frontend calls: GET /api/auth/me (credentials included) to get user info

using System.Security.Claims;
using ImperialBackend.Models;
using ImperialBackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

/* ============================
 * Services
 * ============================ */

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<ImperialDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
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

builder.Services.AddSingleton<GuildMemberSyncService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<GuildMemberSyncService>());

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

static string ToPublicUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url))
        return url;

    return url.Replace("http://authentik-server:9000", "http://localhost:9000")
        .Replace("https://authentik-server:9000", "http://localhost:9000")
        .Replace("http://backend:5032", "http://localhost:5032")
        .Replace("https://backend:5032", "http://localhost:5032");
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
const string publicBackendBaseUrl = "http://localhost:5032";
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
                // Use the public issuer; if your issuer differs, adjust accordingly
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

            options.Events.OnSignedOutCallbackRedirect = context =>
            {
                context.Response.Redirect("http://localhost:5173/");
                context.HandleResponse();
                return Task.CompletedTask;
            };

            options.Events = new OpenIdConnectEvents
            {
                // ============================
                // LOGIN
                // ============================
                OnRedirectToIdentityProvider = context =>
                {
                    // Authentik authorize endpoint
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.IssuerAddress))
                        context.ProtocolMessage.IssuerAddress = ToPublicUrl(
                            context.ProtocolMessage.IssuerAddress
                        );

                    // redirect_uri (login callback)
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.RedirectUri))
                        context.ProtocolMessage.RedirectUri = ToPublicUrl(
                            context.ProtocolMessage.RedirectUri
                        );

                    return Task.CompletedTask;
                },

                // ============================
                // LOGOUT
                // ============================
                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    // Authentik end-session endpoint
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.IssuerAddress))
                        context.ProtocolMessage.IssuerAddress = ToPublicUrl(
                            context.ProtocolMessage.IssuerAddress
                        );

                    // post_logout_redirect_uri
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.PostLogoutRedirectUri))
                        context.ProtocolMessage.PostLogoutRedirectUri = ToPublicUrl(
                            context.ProtocolMessage.PostLogoutRedirectUri
                        );

                    return Task.CompletedTask;
                },

                // ============================
                // AFTER LOGIN CALLBACK
                // ============================
                OnTicketReceived = context =>
                {
                    if (!string.IsNullOrEmpty(context.Properties.RedirectUri))
                        context.Properties.RedirectUri = ToPublicUrl(
                            context.Properties.RedirectUri
                        );

                    return Task.CompletedTask;
                },

                // ============================
                // AFTER LOGOUT CALLBACK
                // ============================
                OnSignedOutCallbackRedirect = context =>
                {
                    context.Response.Redirect("http://localhost:5173/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },

                // ============================
                // FAILURE
                // ============================
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("http://localhost:5173/login?error=auth_failed");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
            };
        }
    );

builder.Services.AddAuthorization();

/* ============================
 * App + Middleware
 * ============================ */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ImperialDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you ever put a reverse proxy in front later, keep this.
// It won’t hurt now, and helps if Host/Proto are forwarded.
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
app.MapControllers();

app.Run();
