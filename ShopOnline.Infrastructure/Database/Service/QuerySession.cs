using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database.Service;

public class QuerySession
{
    internal readonly ApplicationDbContext _context;
    internal QuerySession(ApplicationDbContext context)
    {
        _context = context;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public T? FindByID<T>(Guid Id) where T : EntityBase
    {
        return _context.Set<T>().FirstOrDefault(x => x.Id == Id);
    }

    public async Task<T?> FirstOrDefaultAsync<T>(Func<T, bool> pridicate, Func<T, EntityBase>? Include) where T : EntityBase
    {
        var query = _context.Set<T>().Where(x => pridicate(x));
        if (Include != null)
            query.Include(inc => Include(inc));
        return await query.FirstOrDefaultAsync();
    }

    public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsQueryable();

    public async Task<TValue?> SqlAsync<TValue>(FormattableString sql)
        => await _context.Database.SqlQuery<TValue>(sql).FirstOrDefaultAsync();

    public async Task<TValue?> ExecuteAsync<TValue>(Func<DatabaseFacade, Task<TValue?>> action)
    {
        return await action(_context.Database);
    }

    public virtual async ValueTask DisposeAsync() => await _context.DisposeAsync();
}
