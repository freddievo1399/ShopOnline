using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Server.Entity;
using ShopOnline.Auth.Server.Utils;
using ShopOnline.Auth.Share.Dto.Res;
using ShopOnline.Auth.Share.Interface;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.Infrastructure.Database.Interface;
using ShopOnline.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Service;

public class AuthService(
    IMemoryCache memoryCache,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration config,
    IHttpContextAccessor _httpContextAccessor,
    IDataSessionProvider dataSessionProvider) : IAuth
{
    private string nameAccessTokenCookie => config["Auth:AuthorizationName"];
    private string refeshTokenName => config["Auth:RefeshTokenName"];

    readonly string _key = config["Auth:Key"];
    readonly string _issuer = config["Auth:Issuer"];
    readonly string _audience = config["Auth:Audience"];
    readonly int _expireationInMinutes = config.GetValue<int>("Auth:ExpireationInMinutes");
    readonly int _expireationRefeshTokenInDays = config.GetValue<int>("Auth:ExpireationRefeshTokenInDays");


    public async Task<Result> DisableThisSession()
    {
        try
        {

            var IsAuthHeader = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken);
            string refreshToken;
            bool HasRefreshToken = false;
            if (IsAuthHeader)
            {
                HasRefreshToken = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(refeshTokenName, out var refreshTokenTemp);
                refreshToken = refreshTokenTemp;
            }
            else
            {
                HasRefreshToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(refeshTokenName, out refreshToken);

            }
            if (HasRefreshToken)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(nameAccessTokenCookie);
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(refeshTokenName);
                return await DisableSession(refreshToken);
            }
            return (true, "Realy logout");
        }
        catch (Exception ex)
        {

            return ex.Message;

        }
    }

    public async Task<Result> DisableSession(string refreshToken)
    {
        var query = await dataSessionProvider.OpenQueryAsync();
        var entityRefreshToken = await query.FirstOrDefaultAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken);
        if (entityRefreshToken == null)
        {
            return "Not found";
        }
        if (entityRefreshToken.RefreshTokenExpiration < DateTime.UtcNow)
        {
            using (var transaction = await dataSessionProvider.OpenWorkAsync(refreshToken))
            {
                await transaction.RemoveAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken);
                await transaction.SaveAsync();
            }
            return "Refresh token expired";
        }

        memoryCache.Set($"BlockToken_{entityRefreshToken.AccessToken}", true, TimeSpan.FromHours(1));

        using (var transaction = await dataSessionProvider.OpenWorkAsync(refreshToken))
        {
            await transaction.RemoveAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken);
            await transaction.SaveAsync();
        }

        return true;
    }

    public async Task<ResultOf<AuthRes>> LoginAsync(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            return "User Name sai";
        if (!await userManager.CheckPasswordAsync(user, password))
        {
            return "Sai password";
        }

        var result = await GenerateAuthResultAsync(username);
        try
        {

            using (var transaction = await dataSessionProvider.OpenWorkAsync(result.RefreshToken))
            {
                await transaction.AddAsync<EntityRefreshToken>(new()
                {
                    Id = Guid.NewGuid(),
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    RefreshTokenExpiration = result.RefreshTokenExpiration,
                    UserName = username,
                    RefreshFrom = "login"
                });
                await transaction.SaveAsync();
            }
        }
        catch (Exception ex)
        {

            throw;
        }

        return result;
    }

    public async Task<ResultOf<AuthRes>> RefreshTokenAsync(string refreshToken)
    {
        var query = await dataSessionProvider.OpenQueryAsync();
        var entityRefreshToken = await query.FirstOrDefaultAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiration > DateTime.UtcNow);
        if (entityRefreshToken == null)
        {
            return "Not found";
        }
        if (entityRefreshToken.RefreshTokenExpiration < DateTime.UtcNow)
        {
            using (var transaction = await dataSessionProvider.OpenWorkAsync(refreshToken))
            {
                await transaction.RemoveAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken);
                await transaction.SaveAsync();
            }
            return "Refresh token expired";
        }
        var result = await GenerateAuthResultAsync(entityRefreshToken.UserName);

        memoryCache.Set($"BlockToken_{entityRefreshToken.AccessToken}", true, TimeSpan.FromMinutes(_expireationInMinutes));

        using (var transaction = await dataSessionProvider.OpenWorkAsync(refreshToken))
        {
            await transaction.AddAsync<EntityRefreshToken>(new()
            {
                Id = Guid.NewGuid(),
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RefreshTokenExpiration = result.RefreshTokenExpiration,
                UserName = entityRefreshToken.UserName,
                RefreshFrom = entityRefreshToken.RefreshToken
            });

            await transaction.RemoveAsync<EntityRefreshToken>(x => x.RefreshToken == refreshToken);
            await transaction.SaveAsync();
        }
        return result;
    }

    private async Task<List<Claim>> BuildClaimsAsync(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        claims.Add(new Claim(ClaimTypes.Role, string.Join(",", roles)));
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            var roleClaims = await roleManager.GetClaimsAsync(role!);

            foreach (var rc in roleClaims)
            {
                var existingClaim = claims.FirstOrDefault(x => x.Type == rc.Type);
                if (existingClaim != null)
                {
                    var newValue = ClaimsPrincipalUtils.MergerPermisson(existingClaim.Value, rc.Value).ToString();
                    claims.Remove(existingClaim);
                    claims.Add(new Claim(rc.Type, newValue));
                }
                else
                    claims.Add(rc);
            }
        }

        return claims;
    }
    private (string Token, DateTime Expires) GenerateJwt(List<Claim> claims)
    {
        var expires = DateTime.UtcNow.AddMinutes(_expireationInMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //claims.Add(new(ClaimTypes.Expired, expires.ToString()));
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }
    private string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
    private async Task<AuthRes> GenerateAuthResultAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        var roles = await userManager.GetRolesAsync(user!);
        var claims = await BuildClaimsAsync(user!, roles);
        var (accessToken, accessTokenExpiration) = GenerateJwt(claims);
        var refreshTokenValue = GenerateSecureToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_expireationRefeshTokenInDays);

        return new AuthRes
        {
            AccessToken = accessToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshToken = refreshTokenValue,
            RefreshTokenExpiration = refreshTokenExpiresAt
        };
    }

    public bool IsTokenAllowed(string accessToken)
    {
        memoryCache.TryGetValue($"BlockToken_{accessToken}", out var isBlocked);
        if (isBlocked != null && (bool)isBlocked)
        {
            return false;
        }

        return true;
    }

    public async Task SeedingUser()
    {
        var userName = "Admin";
        var password = "@As123";
        var fullName = "Admin";
        var rolleName = "Admin";
        var userAddmin = await userManager.FindByNameAsync(userName);
        if (userAddmin == null)
        {

            var user = new ApplicationUser { UserName = userName, FullName = fullName };
            try
            {
                var resultCrateUser = await userManager.CreateAsync(user, password);
                if (!resultCrateUser.Succeeded)
                    throw new Exception("Lỗi tạo user mặc định: " + string.Join(", ", resultCrateUser.Errors.Select(e => e.Description)));
                var resultCrateRole = await roleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                if (!resultCrateRole.Succeeded)
                    throw new Exception("Lỗi tạo user mặc định: " + string.Join(", ", resultCrateRole.Errors.Select(e => e.Description)));
                var resultAddToRole = await userManager.AddToRoleAsync(user, rolleName);
                if (!resultAddToRole.Succeeded)
                    throw new Exception("Lỗi thêm user vào role: " + string.Join(", ", resultAddToRole.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo user mặc định", ex);
            }
        }
        var claims = PermissionHub.GetClaims();
        var roleAdmin = await roleManager.FindByNameAsync(rolleName);
        var claimAdmin = await roleManager.GetClaimsAsync(roleAdmin);
        foreach (var claim in claims)
        {
            var claimUpdate = claimAdmin.FirstOrDefault(claimCurrent => claimCurrent.Type == claim.Type);
            if (claimUpdate != null)
            {
                await roleManager.RemoveClaimAsync(roleAdmin, claimUpdate);
            }
            var resultAddToClaim = await roleManager.AddClaimAsync(roleAdmin!, claim);
            if (!resultAddToClaim.Succeeded)
                throw new Exception("Lỗi thêm claim vào role: " + string.Join(", ", resultAddToClaim.Errors.Select(e => e.Description)));
        }
    }
}
