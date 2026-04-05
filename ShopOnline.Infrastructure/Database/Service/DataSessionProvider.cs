using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Infrastructure.Database;
using ShopOnline.Infrastructure.Database.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database.Service
{
    public class DataSessionProvider(IDbContextFactory<ApplicationDbContext> _contextFactory, IHttpContextAccessor httpContextAccessor) : IDataSessionProvider
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _lockPool = new();
        public async Task<QuerySession> OpenQueryAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return new QuerySession(context);
        }

        public async Task<WorkSession> OpenWorkAsync(params string[] lockKeys)
        {
            var sortedKeys = lockKeys.Distinct().OrderBy(k => k).ToList();

            var semaphores = new List<SemaphoreSlim>();

            try
            {
                foreach (var key in sortedKeys)
                {
                    var semaphore = _lockPool.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                    await semaphore.WaitAsync();
                    semaphores.Add(semaphore);
                }

                var context = await _contextFactory.CreateDbContextAsync();
                return new WorkSession(context, httpContextAccessor, lockKeys);
            }
            catch
            {
                foreach (var s in semaphores) s.Release();
                throw;
            }
        }
        internal static void ReleaseLocks(IEnumerable<string> lockKeys)
        {
            foreach (var key in lockKeys)
            {
                if (_lockPool.TryGetValue(key, out var semaphore))
                {
                    semaphore.Release();
                }
            }
        }
    }
}
