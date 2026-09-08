namespace Zoo.Common;

public class Enclosure
{
    // Властивості класу
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public int Capacity { get; set; }

    // Конструктор за замовчуванням
    public Enclosure()
    {
        Id = Guid.NewGuid();
        Name = "Стандартний вольєр";
        Area = 20;
        Capacity = 2;
    }

    // Конструктор з параметрами
    public Enclosure(
        string name,
        double area,
        int capacity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Area = area;
        Capacity = capacity;
    }

    // Метод
    public bool CanAccommodate(int animalCount)
    {
        return animalCount <= Capacity;
    }

    public override string ToString()
    {
        return $"Вольєр: {Name}, площа: {Area} м², місткість: {Capacity}";
    }
}