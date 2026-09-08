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
}