using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly APS _aps;

    public AuthController(ILogger<AuthController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }

    public static async Task<Tokens> PrepareTokens(HttpRequest request, HttpResponse response, APS aps)
    {
        if (!request.Cookies.ContainsKey("access_token"))
        {
            return null;
        }
        var tokens = new Tokens
        {
            AccessToken = request.Cookies["access_token"],
            RefreshToken = request.Cookies["refresh_token"],
            ExpiresAt = DateTime.Parse(request.Cookies["expires_at"])
        };
        if (tokens.ExpiresAt < DateTime.Now.ToUniversalTime())
        {
            tokens = await aps.RefreshTokens(tokens);
            response.Cookies.Append("access_token", tokens.AccessToken);
            response.Cookies.Append("refresh_token", tokens.RefreshToken);
            response.Cookies.Append("expires_at", tokens.ExpiresAt.ToString());
        }
        return tokens;
    }

    [HttpGet("login")]
    public ActionResult Login()
    {
        var redirectUri = _aps.GetAuthorizationURL();
        return Redirect(redirectUri);
    }

    [HttpGet("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        Response.Cookies.Delete("expires_at");
        return Redirect("/");
    }

    [HttpGet("callback")]
    public async Task<ActionResult> Callback(string code)
    {
        var tokens = await _aps.GenerateTokens(code);
        Response.Cookies.Append("access_token", tokens.AccessToken);
        Response.Cookies.Append("refresh_token", tokens.RefreshToken);
        Response.Cookies.Append("expires_at", tokens.ExpiresAt.ToString());
        return Redirect("/");
    }

    [HttpGet("profile")]
    public async Task<dynamic> GetProfile()
    {

        var tokens = await PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        dynamic profile = await _aps.GetUserProfile(tokens);
        return new
        {
            name = profile.Name
        };
    }
}