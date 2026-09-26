namespace Zoo.Common;

public class Mammal : Animal
{
    // Властивості класу-нащадка
    public string FurColor { get; set; }
    public string Diet { get; set; }
    public double Weight { get; set; }

    // Конструктор за замовчуванням
    public Mammal() : base()
    {
        FurColor = "Невідомий";
        Diet = "Невідомий";
        Weight = 0;
    }

    // Конструктор з параметрами
    public Mammal(
        string name,
        string species,
        int age,
        string furColor,
        string diet,
        double weight)
        : base(name, species, age)
    {
        FurColor = furColor;
        Diet = diet;
        Weight = weight;
    }

    // Перевизначення методу
    public override void Feed()
    {
        Console.WriteLine(
            $"Ссавця {Name} нагодовано відповідно до раціону: {Diet}."
        );
    }

    public override string ToString()
    {
        return base.ToString() +
               $", колір шерсті: {FurColor}, раціон: {Diet}, вага: {Weight} кг";
    }

    // Статичний метод для генерації нового випадкового об'єкту
    public static Mammal CreateNew()
    {
        var rnd = Random.Shared;
        var age = rnd.Next(1, 20);
        var weight = Math.Round(rnd.NextDouble() * 200 + 20, 1);
        var names = new[] { "Сімба", "Мурчик", "Ричі", "Лео", "Балу" };
        var species = new[] { "Лев", "Тигр", "Ведмідь", "Пантера" };

        return new Mammal(
            names[rnd.Next(names.Length)],
            species[rnd.Next(species.Length)],
            age,
            "Різний",
            "М'ясоїдний",
            weight
        );
    }
}