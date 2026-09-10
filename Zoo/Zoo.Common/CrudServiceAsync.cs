using System.Text.Json;
using System.Collections;

namespace Zoo.Common;

public class CrudServiceAsync<T> : ICrudServiceAsync<T>
    where T : class, IEntity
{
    private readonly IRepository<T> _repository;

    public string FilePath { get; set; } = "data_async.json";

    public event ZooNotificationHandler? OperationPerformed;

    // Конструктор приймає репозиторій для доступу до БД
    public CrudServiceAsync(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateAsync(T element)
    {
        await _repository.AddAsync(element);
        await _repository.SaveAsync();

        OperationPerformed?.Invoke($"CREATE: додано {element.Name} (ID: {element.Id})");
        return true;
    }

    public async Task<T?> ReadAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

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
        OperationPerformed?.Invoke($"UPDATE: оновлено {element.Name} (ID: {element.Id})");
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        await _repository.DeleteAsync(element);
        await _repository.SaveAsync();
        OperationPerformed?.Invoke($"REMOVE: видалено {element.Name} (ID: {element.Id})");
        return true;
    }

    public async Task<bool> SaveAsync()
    {
        await _repository.SaveAsync();
        OperationPerformed?.Invoke($"SAVE: збережено дані через репозиторій");
        return true;
    }

    // IEnumerable<T> реалізація через отримання всіх елементів (блокуюче на результат)
    public IEnumerator<T> GetEnumerator()
    {
        var all = _repository.GetAllAsync().GetAwaiter().GetResult().ToList();
        return all.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
