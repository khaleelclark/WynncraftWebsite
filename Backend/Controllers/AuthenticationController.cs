using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger;
    }

    private static string Env(IConfiguration config, string key)
    {
        var value = config[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{key} is not set.");
        return value.TrimEnd('/');
    }

    private static string CombineUrl(string baseUrl, string path)
    {
        baseUrl = (baseUrl ?? "").TrimEnd('/');
        path = (path ?? "").Trim();

        if (string.IsNullOrEmpty(path))
            return baseUrl;
        if (!path.StartsWith("/"))
            path = "/" + path;

        return (baseUrl + path).TrimEnd('/');
    }

    /* ============================
     * LOGIN
     * ============================ */
    [EnableRateLimiting("auth")]
    [HttpGet("login")]
    public IActionResult Login([FromServices] IConfiguration config)
    {
        var frontendHome = Env(config, "FRONTEND_URL") + "/";

        return Challenge(
            new AuthenticationProperties { RedirectUri = frontendHome },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    /* ============================
     * LOGOUT
     * ============================ */

    [HttpGet("logout")]
    public IActionResult LogoutWebsiteOnly([FromServices] IConfiguration config)
    {
        var frontendHome = Env(config, "FRONTEND_URL") + "/";

        return SignOut(
            new AuthenticationProperties { RedirectUri = frontendHome },
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    // Full logout (clears imperial.auth cookie + logs out of Authentik SSO)
    [HttpGet("logout-all")]
    public IActionResult LogoutAll([FromServices] IConfiguration config)
    {
        var frontendHome = Env(config, "FRONTEND_URL") + "/";

        return SignOut(
            new AuthenticationProperties { RedirectUri = frontendHome },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    /* ============================
     * WHO AM I
     * ============================ */

    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
            return Unauthorized();

        return Ok(
            new
            {
                isAuthenticated = true,
                name = user.Identity!.Name,
                claims = user.Claims.Select(c => new { c.Type, c.Value }),
            }
        );
    }

    /* ============================
     * AUTH PROVIDER HEALTH
     * ============================ */

    [HttpGet("status")]
    public async Task<IActionResult> Status(
        [FromServices] IHttpClientFactory http,
        [FromServices] IConfiguration config,
        CancellationToken ct
    )
    {
        // Use INTERNAL Authentik URL for backend-to-Authentik communication
        var authentikInternalUrl = Env(config, "AUTHENTIK_INTERNAL_URL");
        var metadata =
            $"{authentikInternalUrl}/application/o/imperial-web/.well-known/openid-configuration";

        _logger.LogInformation("[Auth Status] Checking Authentik at: {MetadataUrl}", metadata);

        try
        {
            var client = http.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            _logger.LogDebug("[Auth Status] Sending request...");
            using var res = await client.GetAsync(metadata, ct);

            _logger.LogInformation("[Auth Status] Response status: {StatusCode}", res.StatusCode);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "[Auth Status] Authentik returned non-success status: {StatusCode}",
                    res.StatusCode
                );
                return StatusCode(503, new { ok = false, error = "AuthProviderUnavailable" });
            }

            _logger.LogInformation("[Auth Status] Authentik is healthy");
            return Ok(new { ok = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[Auth Status] Failed to connect to Authentik at {MetadataUrl}",
                metadata
            );
            return StatusCode(503, new { ok = false, error = "AuthProviderUnavailable" });
        }
    }
}
