using System.Collections;
using System.Linq.Expressions;
using Zoo.Common.Models;

namespace Zoo.Common.Interfaces;

public interface ICrudServiceAsync<T> : IEnumerable<T>
    where T : class, IEntity
{
    Task<bool> CreateAsync(T element);
    Task<T?> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();

    event ZooNotificationHandler? OperationPerformed;
}