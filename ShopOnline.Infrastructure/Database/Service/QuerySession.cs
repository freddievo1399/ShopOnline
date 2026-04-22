using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database.Service;

public class QuerySession : IDisposable
{
    internal readonly ApplicationDbContext _context;
    internal QuerySession(ApplicationDbContext context)
    {
        _context = context;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public async Task<T?> FindByIdAsync<T>(Guid Id) where T : EntityBase
    {
        return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> pridicate, Expression<Func<T, EntityBase>> Include = null) where T : EntityBase
    {
        try
        {

            var query = _context.Set<T>().Where(pridicate);
            if (Include != null)
                query = query.Include(Include);
            return await query.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsQueryable();

    public async Task<TValue?> SqlAsync<TValue>(FormattableString sql)
        => await _context.Database.SqlQuery<TValue>(sql).FirstOrDefaultAsync();

    public async Task<TValue?> ExecuteAsync<TValue>(Func<DatabaseFacade, Task<TValue?>> action)
    {
        return await action(_context.Database);
    }


    public async void Dispose()
    {
        await _context.DisposeAsync();
    }
}
