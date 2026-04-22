using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Abstractions.Constants;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Server.Service;
using ShopOnline.Auth.Share.Interface;
using ShopOnline.BaseBlazor.Interface;

namespace WebApp.Services;

public class AutoDI<T>(T service, IAuth auth, IHttpContextAccessor httpContextAccessor, IConfiguration config, NavigationManager navigation) : IAppBlazorService<T>
{

    private async Task<V> ExcuteInternalAsync<V>(Func<T, Task<V>> func) where V : Result, new()
    {
        var rlt = await func(service);
        if (!rlt.Success)
        {
            switch (rlt.Message)
            {
                case ResultCode.Unauthenticated:
                    var pathAndQuery = new Uri(navigation.Uri).PathAndQuery;
                    string encodedUrl = Uri.EscapeDataString(pathAndQuery);
                    navigation.NavigateTo($"/auth/login/{encodedUrl}", true);
                    break;
                default:
                    break;
            }
        }
        return rlt;
    }
    public async Task<Result> ExcuteAsync(Func<T, Task<Result>> func)
    {
        return await ExcuteInternalAsync(func);
    }

    public async Task<ResultOf<V>> ExcuteAsync<V>(Func<T, Task<ResultOf<V>>> func)
    {
        return await ExcuteInternalAsync(func);
    }

    public async Task<ResultsOf<V>> ExcuteAsync<V>(Func<T, Task<ResultsOf<V>>> func)
    {
        return await ExcuteInternalAsync(func);
    }
    public async Task<PagedResultsOf<V>> ExcuteAsync<V>(Func<T, Task<PagedResultsOf<V>>> func)
    {
        return await ExcuteInternalAsync(func);
    }
}
