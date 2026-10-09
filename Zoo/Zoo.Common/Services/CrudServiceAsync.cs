using System.Collections;
using System.Linq.Expressions;
using Zoo.Common.Interfaces;
using Zoo.Common.Models;

namespace Zoo.Common.Services;

public class CrudServiceAsync<T> : ICrudServiceAsync<T>
    where T : class, IEntity
{
    private readonly IRepository<T> _repository;

    public event ZooNotificationHandler? OperationPerformed;

    public CrudServiceAsync(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateAsync(T element)
    {
        await _repository.AddAsync(element);
        await _repository.SaveAsync();
        OperationPerformed?.Invoke($"CREATE: додано елемент (ID: {element.Id})");
        return true;
    }

    public async Task<T?> ReadAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<T>> ReadAllAsync()
        => await _repository.GetAllAsync();

    // ← ОСЬ ЦЕЙ МЕТОД БУВ ВІДСУТНІЙ
    public async Task<IEnumerable<T>> ReadAllAsync(params Expression<Func<T, object>>[] includes)
        => await _repository.GetAllAsync(includes);

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        var all = (await _repository.GetAllAsync()).ToList();
        if (page < 1) page = 1;
        if (amount < 1) amount = 10;
        return all.Skip((page - 1) * amount).Take(amount).ToList();
    }

    public async Task<bool> UpdateAsync(T element)
    {
        await _repository.UpdateAsync(element);
        await _repository.SaveAsync();
        OperationPerformed?.Invoke($"UPDATE: оновлено елемент (ID: {element.Id})");
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        await _repository.DeleteAsync(element);
        await _repository.SaveAsync();
        OperationPerformed?.Invoke($"REMOVE: видалено елемент (ID: {element.Id})");
        return true;
    }

    public async Task<bool> SaveAsync()
    {
        await _repository.SaveAsync();
        OperationPerformed?.Invoke("SAVE: збережено дані через репозиторій");
        return true;
    }

    public IEnumerator<T> GetEnumerator()
        => _repository.GetAllAsync().GetAwaiter().GetResult().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}