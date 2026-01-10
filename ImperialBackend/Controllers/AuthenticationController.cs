using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    // Needs Secret
    private const string FrontendHome = "http://192.168.4.121:5173/";

    /* ============================
     * LOGIN
     * ============================ */
    [EnableRateLimiting("auth")]
    [HttpGet("login")]
    public IActionResult Login()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = FrontendHome },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    /* ============================
     * LOGOUT
     * ============================ */

    [HttpGet("logout")]
    public IActionResult LogoutWebsiteOnly()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = FrontendHome },
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    // ✅ Full logout (clears imperial.auth cookie + logs out of Authentik SSO)
    [HttpGet("logout-all")]
    public IActionResult LogoutAll()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = FrontendHome },
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

    [HttpGet("status")]
    public async Task<IActionResult> Status(
        [FromServices] IHttpClientFactory http,
        IConfiguration config,
        CancellationToken ct
    )
    {
        // Check if Authentik is up/reachable
        var internalUrl = (
            config["Authentik:InternalUrl"] ?? config["Authentik:Authority"]!
        )!.TrimEnd('/');
        var metadata = $"{internalUrl}/.well-known/openid-configuration";

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
