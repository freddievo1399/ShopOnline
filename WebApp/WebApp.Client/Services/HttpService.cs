using Microsoft.AspNetCore.Components;
using RestEase;
using ShopOnline.Abstractions.Constants;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.BaseBlazor.Interface;

namespace WebApp.Client.Services;

public class HttpService<T>(HttpClient httpClient, NavigationManager navigation) : IAppBlazorService<T>
{
    readonly T _apiClient = RestClient.For<T>(httpClient);
    private async Task<V> ExcuteInternalAsync<V>(Func<T, Task<V>> func) where V : Result, new()
    {

        try
        {
            var rlt = await func(_apiClient);
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
        catch (ApiException ex)
        {
            //ex.StatusCode == 500;
            return new V
            {
                Success = false,
                Message = $"{ex.StatusCode}{ex.Content}_{ex.Message}"
            };
        }
        catch (HttpRequestException ex)
        {
            return new V
            {
                Success = false,
                Message = $"Server / CORS /network:{ex.StatusCode}_{ex.Message}"
            };
        }
        catch (TaskCanceledException ex)
        {
            return new V
            {
                Success = false,
                Message = $"Timeout: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new V
            {
                Success = false,
                Message = $"{ex.Message}"
            };
        }
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
