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
}