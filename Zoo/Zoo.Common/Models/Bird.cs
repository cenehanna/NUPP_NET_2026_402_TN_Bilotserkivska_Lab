namespace Zoo.Common;

public class Bird : Animal
{
    // Властивості класу-нащадка
    public double Wingspan { get; set; }
    public bool CanFly { get; set; }
    public string FeatherColor { get; set; }

    // Конструктор за замовчуванням
    public Bird() : base()
    {
        Wingspan = 0;
        CanFly = true;
        FeatherColor = "Невідомий";
    }

    // Конструктор з параметрами
    public Bird(
        string name,
        string species,
        int age,
        double wingspan,
        bool canFly,
        string featherColor)
        : base(name, species, age)
    {
        Wingspan = wingspan;
        CanFly = canFly;
        FeatherColor = featherColor;
    }

    // Метод
    public void Fly()
    {
        if (CanFly)
        {
            Console.WriteLine($"Птах {Name} може літати.");
        }
        else
        {
            Console.WriteLine($"Птах {Name} не може літати.");
        }
    }

    public override string ToString()
    {
        return base.ToString() +
               $", розмах крил: {Wingspan} м, колір пір'я: {FeatherColor}";
    }

    // Статичний метод для генерації нового випадкового об'єкту
    public static Bird CreateNew()
    {
        var rnd = Random.Shared;
        var age = rnd.Next(1, 15);
        var wingspan = Math.Round(rnd.NextDouble() * 2.5 + 0.2, 2);
        var names = new[] { "Кеша", "Орлик", "Чижик", "Сова", "Папуга" };
        var species = new[] { "Папуга", "Орел", "Сова", "Голуб", "Сокол" };

        return new Bird(
            names[rnd.Next(names.Length)],
            species[rnd.Next(species.Length)],
            age,
            wingspan,
            true,
            "Різнокольоровий"
        );
    }
}