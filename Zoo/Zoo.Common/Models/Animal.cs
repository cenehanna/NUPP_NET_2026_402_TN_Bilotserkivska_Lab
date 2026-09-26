namespace Zoo.Common;

// Делегат для повідомлень про зміни в системі
public delegate void ZooNotificationHandler(string message);

public class Animal : IEntity
{
    // Статичне поле для підрахунку створених тварин
    private static int _totalAnimalsCount;

    // Властивості класу
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public int Age { get; set; }

    // Подія для повідомлення про зміни стану тварини
    public event ZooNotificationHandler? StatusChanged;

    // Статичний конструктор
    static Animal()
    {
        _totalAnimalsCount = 0;
    }

    // Конструктор за замовчуванням
    public Animal()
    {
        Id = Guid.NewGuid();
        Name = "Невідомо";
        Species = "Невідомий вид";
        Age = 0;

        _totalAnimalsCount++;
    }

    // Конструктор з параметрами
    public Animal(string name, string species, int age)
    {
        Id = Guid.NewGuid();
        Name = name;
        Species = species;
        Age = age;

        _totalAnimalsCount++;
    }

    // Метод
    public virtual void Feed()
    {
        Console.WriteLine($"Тварину {Name} нагодовано.");
    }

    // Метод для зміни віку
    public void UpdateAge(int newAge)
    {
        if (newAge < 0)
        {
            throw new ArgumentException("Вік не може бути від'ємним.");
        }

        Age = newAge;

        // Виклик події
        StatusChanged?.Invoke(
            $"Вік тварини \"{Name}\" було змінено на {Age} років."
        );
    }

    // Статичний метод
    public static int GetTotalAnimalsCount()
    {
        return _totalAnimalsCount;
    }

    public override string ToString()
    {
        return $"{Species}: {Name}, {Age} років";
    }
}