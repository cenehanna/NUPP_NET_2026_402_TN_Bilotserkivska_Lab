using System.Text.Json;
using Zoo.Common.Models;

namespace Zoo.Common.Services;

public class CrudService<T> : ICrudService<T>
    where T : Animal
{
    private readonly List<T> _items = new();

    // Конструктор
    public CrudService() { }
    public event ZooNotificationHandler? OperationPerformed;

    public void Create(T element)
    {
        _items.Add(element);

        OperationPerformed?.Invoke(
            $"CREATE: додано тварину \"{element.Name}\" (ID: {element.Id})"
        );
    }

    public T? Read(Guid id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<T> ReadAll()
    {
        return _items;
    }

    public void Update(T element)
    {
        var existingItem = Read(element.Id);

        if (existingItem == null)
        {
            throw new InvalidOperationException(
                "Тварину з таким ID не знайдено."
            );
        }

        var index = _items.IndexOf(existingItem);

        _items[index] = element;

        OperationPerformed?.Invoke(
            $"UPDATE: оновлено тварину \"{element.Name}\" (ID: {element.Id})"
        );
    }

    public void Remove(T element)
    {
        var existingItem = Read(element.Id);

        if (existingItem == null)
        {
            throw new InvalidOperationException(
                "Тварину з таким ID не знайдено."
            );
        }

        _items.Remove(existingItem);

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

        var json = JsonSerializer.Serialize(_items, options);

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
        _items.AddRange(items);

        OperationPerformed?.Invoke(
            $"LOAD: завантажено {items.Count} записів з файлу \"{filePath}\""
        );
    }
}