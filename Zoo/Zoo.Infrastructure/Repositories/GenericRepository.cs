using Microsoft.EntityFrameworkCore;
using Zoo.Common;

namespace Zoo.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly ZooContext _context;
    private readonly DbSet<T> _set;

    public GenericRepository(ZooContext context)
    {
        _context = context;
        _set = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FindAsync(id);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await Task.CompletedTask;
    }
}
