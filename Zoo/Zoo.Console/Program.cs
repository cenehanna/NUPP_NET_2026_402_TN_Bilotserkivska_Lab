using Zoo.Common.Extensions;
using Zoo.Common.Models;
using Zoo.Common.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==============================================");
Console.WriteLine("              ZOO MANAGEMENT SYSTEM");
Console.WriteLine("==============================================");
// Створення CRUD сервісу
var mammalService = new CrudService<Mammal>();
mammalService.OperationPerformed += OnOperationPerformed; // Делегат та подія
// Створення об'єктів
var lion = new Mammal(
    "Сімба",
    "Африканський лев",
    5,
    "Золотистий",
    "Хижак",
    190
);

var tiger = new Mammal(
    "Річі",
    "Бенгальський тигр",
    7,
    "Помаранчевий з чорними смугами",
    "Хижак",
    210
);


// CREATE
Console.WriteLine("--- CREATE ---");

mammalService.Create(lion);
mammalService.Create(tiger);


// READ ALL
Console.WriteLine("\n--- READ ALL ---");

foreach (var animal in mammalService.ReadAll())
{
    Console.WriteLine(animal);
}


// READ
Console.WriteLine("\n--- READ ---");

var foundAnimal = mammalService.Read(lion.Id);

if (foundAnimal != null)
{
    Console.WriteLine("Знайдено:");
    Console.WriteLine(foundAnimal);
}


// UPDATE
Console.WriteLine("\n--- UPDATE ---");

lion.Age = 6;
lion.Weight = 195;

mammalService.Update(lion);

Console.WriteLine("Після оновлення:");
Console.WriteLine(mammalService.Read(lion.Id));


// Метод та подія класу Animal
Console.WriteLine("\n--- МЕТОД ТА ПОДІЯ ANIMAL ---");

lion.StatusChanged += OnAnimalStatusChanged;

lion.UpdateAge(7);


// Статичний метод
Console.WriteLine("\n--- STATIC ---");

Console.WriteLine(
    $"Кількість створених тварин: {Animal.GetTotalAnimalsCount()}"
);


// Метод розширення
Console.WriteLine("\n--- EXTENSION METHOD ---");

Console.WriteLine(lion.GetShortInfo());


// Робота Bird
Console.WriteLine("\n--- BIRD ---");

var eagle = new Bird(
    "Орлик",
    "Беркут",
    4,
    2.2,
    true,
    "Темно-коричневий"
);

Console.WriteLine(eagle);

eagle.Fly();

Console.WriteLine(
    $"Коротка інформація: {eagle.GetShortInfo()}"
);


// Робота Enclosure
Console.WriteLine("\n--- ENCLOSURE ---");

var enclosure = new Enclosure(
    "Вольєр №1",
    120,
    3
);

Console.WriteLine(enclosure);

Console.WriteLine(
    $"Чи можна розмістити 2 тварини: " +
    $"{enclosure.CanAccommodate(2)}"
);


// SAVE
Console.WriteLine("\n--- SAVE ---");

const string filePath = "mammals.json";

mammalService.Save(filePath);


// LOAD
Console.WriteLine("\n--- LOAD ---");

var loadedService = new CrudService<Mammal>();

loadedService.OperationPerformed += OnOperationPerformed;

loadedService.Load(filePath);

Console.WriteLine("Завантажені дані:");

foreach (var animal in loadedService.ReadAll())
{
    Console.WriteLine(animal);
}


// REMOVE
Console.WriteLine("\n--- REMOVE ---");

mammalService.Remove(tiger);

Console.WriteLine(
    $"Кількість записів після видалення: " +
    $"{mammalService.ReadAll().Count()}"
);


Console.WriteLine();
Console.WriteLine("==============================================");
Console.WriteLine("        Виконання лабораторної завершено");
Console.WriteLine("==============================================");
static void OnOperationPerformed(string message)
{
    Console.WriteLine($"[LOG] {message}");
}

static void OnAnimalStatusChanged(string message)
{
    Console.WriteLine($"[EVENT] {message}");
}