using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database.Service;

public class WorkSession : QuerySession
{
    private IDbContextTransaction _transaction;
    private IEnumerable<string> _lockKeys;
    IHttpContextAccessor _httpContextAccessor;
    private bool _isCommitted = false;
    ClaimsPrincipal User => _httpContextAccessor!.HttpContext!.User;
    internal WorkSession(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, params string[] lockKeys)
        : base(context)
    {
        _lockKeys = lockKeys;
        _httpContextAccessor = httpContextAccessor;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        _transaction ??= _context.Database.BeginTransaction();
    }

    public T Add<T>(T entity) where T : EntityBase
    {
        var tempEntity = FindByID<T>(entity.Id);
        if (tempEntity != null)
        {
            throw new InvalidOperationException($"Entity with ID {entity.Id} already exists.");
        }
        _context.Set<T>().Add(entity);
        return entity;
    }


    public void Remove<T>(T entity) where T : EntityBase
    {
        entity.IsDeleted = true;
        entity.DeletedBy = User.GetUserName();
        entity.DeletedAt = DateTime.UtcNow;
    }

    public void RemoveById<T>(Guid Id) where T : EntityBase
    {
        var entity = FindByID<T>(Id);
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with ID {Id} does not exist.");
        }
        Remove(entity);
    }
    public void RemoveRange<T>(IEnumerable<T> entities) where T : EntityBase
    {
        foreach (var entity in entities)
        {
            Remove(entity);
        }
    }

    public async Task SaveAsync()
    {

        _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
        .ToList()
        .ForEach(e => e.State = e.State); // test log

        await _context.SaveChangesAsync();
        await _transaction.CommitAsync();

        _isCommitted = true;
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            // Nếu chưa commit mà đã dispose (có thể do lỗi giữa chừng)
            if (!_isCommitted && _transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            // Luôn giải phóng tài nguyên
            DataSessionProvider.ReleaseLocks(_lockKeys);
            if (_transaction != null) await _transaction.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
