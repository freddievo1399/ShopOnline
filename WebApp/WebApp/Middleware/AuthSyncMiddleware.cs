using Azure.Core;
using ShopOnline.Auth.Share.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace WebApp.Middleware;
/// <summary>
/// Middleware đồng bộ giữa header Authorization và cookie 
/// Update jwt qua RefeshToken thì sẽ update cả cookie và header để tránh trường hợp 1 trong 2 bị hết hạn mà không đồng bộ được
/// </summary>
public class AuthSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private string nameAccessTokenCookie => _config["Auth:AuthorizationName"];
    private string refeshTokenName => _config["Auth:RefeshTokenName"];

    public AuthSyncMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        var authService = context.RequestServices.GetService<IAuth>();
        var IsAuthHeader = context.Request.Headers.TryGetValue("Authorization", out var accessToken);
        var IsAuthCookie = context.Request.Cookies.TryGetValue(nameAccessTokenCookie, out var authCookieToken);

        if (IsAuthCookie)
        {
            context.Request.Headers.TryAdd("Authorization", $"Bearer {authCookieToken}");
            accessToken = authCookieToken;
        }
        if (!authService!.IsTokenAllowed(authCookieToken!))
        {
            context.Request.Headers.Remove("Authorization");
            context.Response.Cookies.Delete(nameAccessTokenCookie);
            context.Response.Cookies.Delete(refeshTokenName);
            context.Response.Headers.Remove("Authorization");
            await _next(context);
            return;
        }
        if (IsAuthHeader || IsAuthCookie)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                //httponly
                if (IsAuthHeader)
                {
                    var isRefreshToken = context.Request.Headers.TryGetValue(refeshTokenName, out var refreshToken);
                    if (isRefreshToken)
                    {
                        var authResult = await authService.RefreshTokenAsync(refreshToken);
                        if (authResult.Success)
                        {

                            context.Response.Headers.TryAdd("Authorization", $"Bearer {authResult.Item.AccessToken}");
                            context.Response.Headers.TryAdd(refeshTokenName, $"{authResult.Item.RefreshToken}");
                        }
                    }

                }
                else
                {
                    var isRefreshToken = context.Request.Cookies.TryGetValue(refeshTokenName, out var refreshToken);
                    if (isRefreshToken)
                    {
                        var authResult = await authService.RefreshTokenAsync(refreshToken);
                        if (authResult.Success)
                        {
                            context.Response.Cookies.Append(nameAccessTokenCookie, $"{authResult.Item.AccessToken}", new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = authResult.Item.AccessTokenExpiration,
                            });

                            context.Response.Cookies.Append(refeshTokenName, $"{authResult.Item.RefreshToken}", new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = authResult.Item.RefreshTokenExpiration,
                            });
                        }
                    }
                }
            }

        }
        await _next(context);
    }
}
