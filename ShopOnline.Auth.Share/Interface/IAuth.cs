using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Share.Dto.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Interface;

public interface IAuth
{
    Task<ResultOf<AuthRes>> LoginAsync(string username, string password);
    Task<ResultOf<AuthRes>> RefreshTokenAsync(string refreshToken);
    Task<Result> DisableSession(string refreshToken);
    Task<Result> DisableThisSession();
    Task SeedingUser();
    bool IsTokenAllowed(string accessToken);
}
