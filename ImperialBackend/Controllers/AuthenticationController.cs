using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
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
        // In Docker, always hit Authentik by service name.
        // (Browser never sees this; it's only your backend checking reachability.)
        const string authentikInternalBase = "http://authentik-server:9000";

        var issuerPath = Env(config, "AUTHENTIK_ISSUER_PATH"); // e.g. /application/o/imperial-web/
        var internalIssuer = CombineUrl(authentikInternalBase, issuerPath);
        var metadata = $"{internalIssuer}/.well-known/openid-configuration";

        try
        {
            var client = http.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(2);

            using var res = await client.GetAsync(metadata, ct);
            if (!res.IsSuccessStatusCode)
                return StatusCode(503, new { ok = false, error = "AuthProviderUnavailable" });

            return Ok(new { ok = true });
        }
        catch
        {
            return StatusCode(503, new { ok = false, error = "AuthProviderUnavailable" });
        }
    }
}
