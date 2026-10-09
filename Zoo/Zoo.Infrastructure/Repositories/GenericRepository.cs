using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Zoo.Common.Interfaces;

namespace Zoo.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly ZooContext _context;
    protected readonly DbSet<T> _set;

    public GenericRepository(ZooContext context)
    {
        _context = context;
        _set = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _set;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _set.Attach(entity);
            entry.State = EntityState.Modified;
        }

        // Не чіпаємо навігаційні колекції — EF Core сам розбереться
        foreach (var nav in entry.Navigations)
        {
            if (nav.Metadata.IsCollection)
            {
                continue;
            }
            nav.IsModified = false;
        }

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}