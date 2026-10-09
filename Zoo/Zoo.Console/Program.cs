using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Zoo.Common.Interfaces;
using Zoo.Common.Models;
using Zoo.Common.Services;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Models;
using Zoo.Infrastructure.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("==============================================");
Console.WriteLine("    ДЕМОНСТРАЦІЯ АСИНХРОННОГО CRUD ДЛЯ ЗООПАРКУ");
Console.WriteLine("==============================================");

string connectionString = "Host=localhost;Port=5432;Database=ZooDb;Username=postgres;Password=8gyrg8104ife";

var options = new DbContextOptionsBuilder<ZooContext>()
    .UseNpgsql(connectionString)
    .Options;

using var db = new ZooContext(options);

// === Репозиторії та сервіси (усі — GenericRepository) ===
var mammalRepo = new GenericRepository<MammalModel>(db);
var mammalService = new CrudServiceAsync<MammalModel>(mammalRepo);
mammalService.OperationPerformed += OnOperationPerformed;

var enclosureRepo = new GenericRepository<EnclosureModel>(db);
var enclosureService = new CrudServiceAsync<EnclosureModel>(enclosureRepo);
enclosureService.OperationPerformed += OnOperationPerformed;

var birdRepo = new GenericRepository<BirdModel>(db);
var birdService = new CrudServiceAsync<BirdModel>(birdRepo);
birdService.OperationPerformed += OnOperationPerformed;

var keeperRepo = new GenericRepository<KeeperModel>(db);
var keeperService = new CrudServiceAsync<KeeperModel>(keeperRepo);
keeperService.OperationPerformed += OnOperationPerformed;

var detailRepo = new GenericRepository<AnimalDetailModel>(db);
var detailService = new CrudServiceAsync<AnimalDetailModel>(detailRepo);

var rnd = Random.Shared;

// 1. Вольєри
for (int i = 1; i <= 6; i++)
{
    var enc = EnclosureModel.CreateNew();
    enc.Name = $"Вольєр {i}";
    await enclosureService.CreateAsync(enc);
}
var enclosures = (await enclosureService.ReadAllAsync()).ToList();

// 2. Доглядачі
for (int i = 1; i <= 6; i++)
{
    var k = new KeeperModel { Id = Guid.NewGuid(), Name = $"Keeper {i}" };
    await keeperService.CreateAsync(k);
}
var keepers = (await keeperService.ReadAllAsync()).ToList();

// 3. Птахи
for (int i = 1; i <= 6; i++)
{
    var b = BirdModel.CreateNew();
    b.Name = $"Bird {i}";
    if (enclosures.Count > 0)
    {
        b.EnclosureId = enclosures[rnd.Next(enclosures.Count)].Id;
    }
    await birdService.CreateAsync(b);
}

Console.WriteLine("Додано початкові записи до Enclosures, Keepers, Birds.");

// 4. Ссавці
const int totalToCreate = 6;
Console.WriteLine($"Створюємо {totalToCreate} ссавців...");
for (int i = 0; i < totalToCreate; i++)
{
    var m = MammalModel.CreateNew();
    if (enclosures.Count > 0)
    {
        m.EnclosureId = enclosures[rnd.Next(enclosures.Count)].Id;
    }
    await mammalService.CreateAsync(m);
}
Console.WriteLine($"Створено {totalToCreate} ссавців.");

// 5. Деталі + доглядачі — ТЕПЕР з includes
var allMammals = (await mammalService.ReadAllAsync(
    m => m.Keepers,
    m => m.Detail,
    m => m.Enclosure
)).ToList();

foreach (var mammal in allMammals)
{
    // 1:1 — деталі
    var detail = new AnimalDetailModel
    {
        Id = mammal.Id,
        MedicalNotes = $"Medical notes for {mammal.Name}"
    };
    await detailService.CreateAsync(detail);

    // N:N — доглядачі
    var keeperCount = Math.Min(keepers.Count, rnd.Next(1, 4));
    for (int k = 0; k < keeperCount; k++)
    {
        var keeper = keepers[rnd.Next(keepers.Count)];
        if (!mammal.Keepers.Any(x => x.Id == keeper.Id))
        {
            mammal.Keepers.Add(keeper);
        }
    }

    await mammalService.UpdateAsync(mammal);
}

// Статистика
var minAge = allMammals.Min(a => a.Age);
var maxAge = allMammals.Max(a => a.Age);
var avgWeight = allMammals.Average(a => a.Weight);
Console.WriteLine($"Мін. вік: {minAge}, Макс. вік: {maxAge}, Середня вага: {avgWeight:F2} кг");

// Пагінація
Console.WriteLine("\n--- Демонстрація пагінації (Сторінка 1, Кількість 5) ---");
var pagedList = await mammalService.ReadAllAsync(page: 1, amount: 5);
foreach (var mammal in pagedList)
{
    Console.WriteLine(mammal);
}

// SemaphoreSlim
using var sem = new SemaphoreSlim(3);
var bag = new ConcurrentBag<string>();
var tasks = allMammals.Take(20).Select(async a =>
{
    await sem.WaitAsync();
    try
    {
        await Task.Delay(10);
        bag.Add(a.Name ?? "Unknown");
    }
    finally
    {
        sem.Release();
    }
});
await Task.WhenAll(tasks);
Console.WriteLine($"Зібрано імен: {bag.Count} (SemaphoreSlim).");

// AutoResetEvent
using var autoEvent = new AutoResetEvent(false);
var saveTask = mammalService.SaveAsync();
_ = saveTask.ContinueWith(_ => autoEvent.Set());
Console.WriteLine("Очікування завершення асинхронного збереження...");
autoEvent.WaitOne();
Console.WriteLine("Збереження завершено.");
Console.WriteLine("==============================================");

static void OnOperationPerformed(string message)
{
    Console.WriteLine($"[LOG] {message}");
}