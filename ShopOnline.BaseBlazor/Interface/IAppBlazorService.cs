using ShopOnline.Abstractions.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.BaseBlazor.Interface
{
    public interface IAppBlazorService<T>
    {
        public Task<Result> ExcuteAsync(Func<T, Task<Result>> func);

        Task<ResultOf<V>> ExcuteAsync<V>(Func<T, Task<ResultOf<V>>> func);

        public Task<ResultsOf<V>> ExcuteAsync<V>(Func<T, Task<ResultsOf<V>>> func);

        public Task<PagedResultsOf<V>> ExcuteAsync<V>(Func<T, Task<PagedResultsOf<V>>> func);

        //public Task<ClaimsPrincipal> GetUser();
    }
}
