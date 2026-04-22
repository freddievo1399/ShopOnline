using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Share.Dto.Req;
using ShopOnline.Auth.Share.Dto.Res;
using ShopOnline.Auth.Share.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Controller;

[ApiController]
[Route("/api/Auth/[controller]")]
[ApiExplorerSettings(GroupName = "Auth")]
public class AuthController(IHttpContextAccessor httpContextAccessor, IAuth auth, IConfiguration config) : ControllerBase
{
    private readonly string nameAccessTokenCookie = config["Auth:AuthorizationName"];
    private readonly string refreshTokenName = config["Auth:RefeshTokenName"];
    [HttpPost(nameof(Login))]
    public async Task<ResultOf<AuthRes>> Login([FromBody] LoginReq login)
    {
        var result = await auth.LoginAsync(login.UserName, login.Password);
        if (!result.Success)
        {
            return result.Message;
        }
        var autuResult = result.Item;
        if (result.Success)
        {
            var authResult = result.Item;
            httpContextAccessor.HttpContext.Response.Cookies.Append(nameAccessTokenCookie, $"{autuResult.AccessToken}", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = autuResult.AccessTokenExpiration,
            });

            httpContextAccessor.HttpContext.Response.Cookies.Append(refreshTokenName, $"{autuResult.RefreshToken}", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = autuResult.RefreshTokenExpiration,
            }); ;
        }
        return result;
    }
    [HttpGet(nameof(ReLogin))]
    public async Task<ResultOf<AuthRes>> ReLogin(string refreshToken)
    {
        var result = await auth.RefreshTokenAsync(refreshToken);
        if (!result.Success)
        {
            return result.Message;
        }
        var autuResult = result.Item;
        if (result.Success)
        {
            var authResult = result.Item;
            httpContextAccessor.HttpContext.Response.Cookies.Append(nameAccessTokenCookie, $"{autuResult.AccessToken}", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = autuResult.AccessTokenExpiration,
            });

            httpContextAccessor.HttpContext.Response.Cookies.Append(refreshTokenName, $"{autuResult.RefreshToken}", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = autuResult.RefreshTokenExpiration,
            }); ;
        }
        return result;
    }
    [HttpGet(nameof(LogOut))]
    public async Task<Result> LogOut()
    {
        return await auth.DisableThisSession();
    }

}
