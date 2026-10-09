using System.Text.Json;
using System.Collections;
using Zoo.Common.Models;



namespace Zoo.Common.Services;

public class CrudServiceAsync<T> : ICrudServiceAsync<T>
    where T : Animal
{
    private readonly List<T> _items = new();
    private readonly object _sync = new(); // об'єкт lock для потокобезпечності
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1); // семафор для асинхронного збереження

    public string FilePath { get; set; } = "data_async.json";

    public event ZooNotificationHandler? OperationPerformed;

    public async Task<bool> CreateAsync(T element)
    {
        lock (_sync)
        {
            _items.Add(element);
        }

        OperationPerformed?.Invoke($"CREATE: added {element.Name} (ID: {element.Id})");
        await Task.CompletedTask;
        return true;
    }

    public async Task<T?> ReadAsync(Guid id)
    {
        T? item;
        lock (_sync)
        {
            item = _items.FirstOrDefault(x => x.Id == id);
        }

        await Task.CompletedTask;
        return item;
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        List<T> snapshot;
        lock (_sync)
        {
            snapshot = new List<T>(_items);
        }

        return Task.FromResult<IEnumerable<T>>(snapshot);
    }

    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1) page = 1;
        if (amount < 1) amount = 10;

        List<T> snapshot;
        lock (_sync)
        {
            snapshot = new List<T>(_items);
        }

        var result = snapshot.Skip((page - 1) * amount).Take(amount).ToList();
        return Task.FromResult<IEnumerable<T>>(result);
    }

    public async Task<bool> UpdateAsync(T element)
    {
        lock (_sync)
        {
            var idx = _items.FindIndex(x => x.Id == element.Id);
            if (idx == -1) return false;
            _items[idx] = element;
        }

        OperationPerformed?.Invoke($"UPDATE: updated {element.Name} (ID: {element.Id})");
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        bool removed = false;
        lock (_sync)
        {
            removed = _items.RemoveAll(x => x.Id == element.Id) > 0;
        }

        if (removed)
        {
            OperationPerformed?.Invoke($"REMOVE: removed {element.Name} (ID: {element.Id})");
        }

        await Task.CompletedTask;
        return removed;
    }

    public async Task<bool> SaveAsync()
    {
        await _saveSemaphore.WaitAsync();
        try
        {
            List<T> snapshot;
            lock (_sync)
            {
                snapshot = new List<T>(_items);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(snapshot, options);
            await File.WriteAllTextAsync(FilePath, json);

            OperationPerformed?.Invoke($"SAVE: saved {snapshot.Count} records to '{FilePath}'");
            return true;
        }
        finally
        {
            _saveSemaphore.Release();
        }
    }

    // Реалізація IEnumerable<T> — повернути перерахувач знімка
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;
        lock (_sync)
        {
            snapshot = new List<T>(_items);
        }

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
