using System.Text.Json;
using Zoo.Common.Models;

namespace Zoo.Common.Services;

public class CrudService<T> : ICrudService<T>
    where T : Animal
{
    private readonly Dictionary<Guid, T> _items = new();

    // Конструктор
    public CrudService() { }
    public event ZooNotificationHandler? OperationPerformed;

    public void Create(T element)
    {
        if (_items.ContainsKey(element.Id))
        {
            throw new InvalidOperationException(
                $"Тварину з ID {element.Id} вже додано."
            );
        }

        _items[element.Id] = element;

        OperationPerformed?.Invoke(
            $"CREATE: додано тварину \"{element.Name}\" (ID: {element.Id})"
        );
    }

    public T? Read(Guid id)
    {
        _items.TryGetValue(id, out var item);
        return item;
    }

    public IEnumerable<T> ReadAll()
    {
        return _items.Values;
    }

    public void Update(T element)
    {
        if (!_items.ContainsKey(element.Id))
        {
            throw new InvalidOperationException(
                "Тварину з таким ID не знайдено."
            );
        }

        _items[element.Id] = element;

        OperationPerformed?.Invoke(
            $"UPDATE: оновлено тварину \"{element.Name}\" (ID: {element.Id})"
        );
    }

    public void Remove(T element)
    {
        if (!_items.Remove(element.Id))
        {
            throw new InvalidOperationException(
                "Тварину з таким ID не знайдено."
            );
        }

        OperationPerformed?.Invoke(
            $"REMOVE: видалено тварину \"{element.Name}\" (ID: {element.Id})"
        );
    }

    public void Save(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(_items.Values.ToList(), options);

        File.WriteAllText(filePath, json);

        OperationPerformed?.Invoke(
            $"SAVE: дані збережено у файл \"{filePath}\""
        );
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Файл із даними не знайдено.",
                filePath
            );
        }

        var json = File.ReadAllText(filePath);

        var items = JsonSerializer.Deserialize<List<T>>(json);

        if (items == null)
        {
            return;
        }

        _items.Clear();
        foreach (var item in items)
        {
            _items[item.Id] = item;
        }

        OperationPerformed?.Invoke(
            $"LOAD: завантажено {items.Count} записів з файлу \"{filePath}\""
        );
    }
}