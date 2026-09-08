using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using Zoo.Common;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==============================================");
Console.WriteLine("    ДЕМОНСТРАЦІЯ АСИНХРОННОГО CRUD ДЛЯ ЗООПАРКУ");
Console.WriteLine("==============================================");

// Асинхронний CRUD сервіс (багатопотоково-безпечний)
var mammalService = new CrudServiceAsync<Mammal>
{
    FilePath = "mammals_async.json"
};

mammalService.OperationPerformed += OnOperationPerformed;

// Паралельне створення за допомогою Parallel.ForEachAsync
const int totalToCreate = 5000;
Console.WriteLine($"Створюємо {totalToCreate} ссавців паралельно...");

// Використання lock для локального лічильника
var createdCount = 0;
var counterLock = new object();

await Parallel.ForEachAsync(
    Enumerable.Range(0, totalToCreate),
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
    async (i, ct) =>
    {
        // Генерація об'єкта
        var m = Mammal.CreateNew();

        // Додавання в сервіс
        await mammalService.CreateAsync(m);

        // Оновлення локального лічильника під lock
        lock (counterLock)
        {
            createdCount++;
        }
    }
);

Console.WriteLine($"Створено (за лічильником): {createdCount}");

// Зчитати всі та обчислити статистику
var all = (await mammalService.ReadAllAsync()).ToList();

var minAge = all.Min(a => a.Age);
var maxAge = all.Max(a => a.Age);
var avgWeight = all.Average(a => a.Weight);

Console.WriteLine($"Мін. вік: {minAge}, Макс. вік: {maxAge}, Середня вага: {avgWeight:F2} кг");

// Демонстрація пагінації (наприклад: сторінка 1, 5 елементів)
Console.WriteLine("\n--- Демонстрація пагінації (Сторінка 1, Кількість 5) ---");
var pagedList = await mammalService.ReadAllAsync(page: 1, amount: 5);
foreach (var mammal in pagedList)
{
    Console.WriteLine(mammal);
}

// Демонстрація SemaphoreSlim для обмеження паралельних операцій
using var sem = new SemaphoreSlim(3);
var bag = new ConcurrentBag<string>();

var tasks = all.Take(20).Select(async a =>
{
    await sem.WaitAsync();
    try
    {
        await Task.Delay(10);
        bag.Add(a.Name);
    }
    finally
    {
        sem.Release();
    }
});

await Task.WhenAll(tasks);
Console.WriteLine($"Зібрано імен: {bag.Count} (SemaphoreSlim).");

// Демонстрація AutoResetEvent для сигналізації про завершення збереження
using var autoEvent = new AutoResetEvent(false);
var saveTask = mammalService.SaveAsync();
saveTask.ContinueWith(t => autoEvent.Set());

Console.WriteLine("Очікування завершення асинхронного збереження...");
autoEvent.WaitOne();
Console.WriteLine("Збереження завершено.");

Console.WriteLine("==============================================");

static void OnOperationPerformed(string message)
{
    Console.WriteLine($"[LOG] {message}");
}
