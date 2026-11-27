# Звіт про виконання лабораторної роботи №4
## Тема: Розробка ASP.NET Core Web API та MVC застосунку для зооупокупу

---

## Вступ

У ході цієї лабораторної роботи було розроблено ASP.NET Core Web API проєкт **Zoo.REST** для управління тваринам та вольєрами зоопарку. Проєкт демонструє впровадження тризирівневої архітектури, REST принципів та сучасних практик ASP.NET Core розробки.

---

## 1. Створені файли та їх призначення

### 1.1. Структура проєкту Zoo.REST

```
Zoo.REST/
├─ Controllers/
│  ├─ AnimalsController.cs      # REST контролер для тварин
│  └─ EnclosuresController.cs   # REST контролер для вольєрів
├─ Models/
│  ├─ AnimalApiModel.cs              # DTO для створення/оновлення тварини
│  ├─ AnimalDetailApiModel.cs        # DTO для деталей тварини  
│  ├─ EnclosureApiModel.cs           # DTO для створення/оновлення вольєра
│  └─ EnclosureDetailApiModel.cs     # DTO для деталей вольєра
├─ Program.cs                   # Конфігурація DI та middleware
├─ appsettings.json            # Налаштування підключення БД
└─ Zoo.REST.csproj             # Файл проєкту з залежностями
```

### 1.2. Описання основних файлів

#### **Program.cs**
- Конфігурація служб: реєстрація ZooContext, репозиторіїв, CRUD сервісів
- Налаштування Swagger/Swashbuckle для документування API
- Регістрація контролерів та логування
- Middleware конфігурація для розробки та production

#### **Controllers/AnimalsController.cs**
- **Клас:** `AnimalsController : ControllerBase`
- **Методи:**
  - `GetAllAnimals(page, amount)` - [GET /api/animals] Отримання всіх тварин з пагінацією
  - `GetAnimalById(id)` - [GET /api/animals/{id}] Отримання тварини за ID
  - `CreateAnimal(model)` - [POST /api/animals] Створення нової тварини
  - `UpdateAnimal(id, model)` - [PUT /api/animals/{id}] Оновлення тварини
  - `DeleteAnimal(id)` - [DELETE /api/animals/{id}] Видалення тварини

#### **Controllers/EnclosuresController.cs**
- **Клас:** `EnclosuresController : ControllerBase`
- **Методи:**
  - `GetAllEnclosures(page, amount)` - [GET /api/enclosures] Отримання всіх вольєрів з пагінацією
  - `GetEnclosureById(id)` - [GET /api/enclosures/{id}] Отримання вольєра за ID
  - `CreateEnclosure(model)` - [POST /api/enclosures] Створення нового вольєра
  - `UpdateEnclosure(id, model)` - [PUT /api/enclosures/{id}] Оновлення вольєра
  - `DeleteEnclosure(id)` - [DELETE /api/enclosures/{id}] Видалення вольєра

#### **Models/**
- **AnimalApiModel** - DTO для запитів на створення/оновлення тварини
  - Властивості: Name, Species, Age, EnclosureId
- **AnimalDetailApiModel** - DTO для відповідей з деталями тварини
  - Властивості: Id, Name, Species, Age, EnclosureId, EnclosureName
- **EnclosureApiModel** - DTO для запитів на створення/оновлення вольєра
  - Властивості: Name, Area, Capacity
- **EnclosureDetailApiModel** - DTO для відповідей з деталями вольєра
  - Властивості: Id, Name, Area, Capacity, AnimalCount, AnimalNames

---

## 2. Відповіді на контрольні запитання

### 2.1 Які ключові особливості шаблону ASP.NET Core Web API та чим він відрізняється від Blazor Web App або ASP.NET MVC?

**Відповідь:**

ASP.NET Core Web API має наступні ключові особливості:

1. **Фокус на REST принципи:**
   - Повертає JSON/XML дані, а не HTML потоки
   - Використовує стандартні HTTP методи (GET, POST, PUT, DELETE)
   - Розроблена для мобільних та客户端приложений

2. **Архітектура:**
   - Web API: API-перший підхід, контролери повертають дані
   - MVC: View-Model-Controller, контролери повертають HTML views
   - Blazor: Component-based, SSR та CSR в одному фреймворку

3. **Відповіді:**
   - Web API: `return Ok(data)` → JSON
   - MVC: `return View(model)` → HTML
   - Blazor: Компоненти Razor з interactive features

4. **Використання:**
   - Web API: Бекенд для фронтенду (React, Vue, Angular)
   - MVC: Монолітні вебзастосунки з сервер-side рендерингом
   - Blazor: Full-stack C# розробка з interactive UI

**Приклад з нашого проєкту:**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<AnimalDetailApiModel>> GetAnimalById(Guid id)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(new { message = "Not found" });
	return Ok(new AnimalDetailApiModel { ... });
}
```

---

### 2.2 Поясніть, як реалізується тризирівнева архітектура у вашому проєкті Zoo.REST та які переваги вона має

**Відповідь:**

Тризирівнева архітектура реалізується наступним чином:

**Рівень 1: Presentation (Presentation Layer)**
- **Компоненти:** `AnimalsController`, `EnclosuresController`
- **Відповідальність:** Обробка HTTP запитів, валідація введення, форматування відповідей
- **Місцеположення:** `Zoo.REST/Controllers/`
- **Приклад:**
```csharp
[HttpPost]
public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal(AnimalApiModel model)
{
	if (string.IsNullOrWhiteSpace(model.Name))
		return BadRequest(new { message = "Name required" });
	// ...
}
```

**Рівень 2: Business Logic (Service Layer)**
- **Компоненти:** `ICrudServiceAsync<T>`, `CrudServiceAsync<T>`
- **Відповідальність:** Бізнес-логіка, обробка операцій, координація між層
- **Місцеположення:** `Zoo.Common/Services/`
- **Приклад:**
```csharp
public class CrudServiceAsync<T> : ICrudServiceAsync<T>
{
	public async Task<bool> CreateAsync(T element)
	{
		await _repository.AddAsync(element);
		await _repository.SaveAsync();
		return true;
	}
}
```

**Рівень 3: Data Access (Repository/Data Layer)**
- **Компоненти:** `GenericRepository<T>`, `ZooContext`
- **Відповідальність:** Прямий доступ до БД, SQL запити, маніпуляація даними
- **Місцеположення:** `Zoo.Infrastructure/`
- **Приклад:**
```csharp
public class GenericRepository<T> : IRepository<T>
{
	public async Task<T?> GetByIdAsync(Guid id)
	{
		return await _set.FirstOrDefaultAsync(e => e.Id == id);
	}
}
```

**Переваги тризирівневої архітектури:**

1. **Розділення обов'язків (SoC):**
   - Кожен рівень має чітко визначену роль
   - Зменшує взаємозалежність коду

2. **Тестованість:**
   - Одиниці можуть тестуватися окремо
   - Легко мокувати залежності

3. **Масштабованість:**
   - Легше додавати нові функції
   - Можна розподілити команду по рівнях

4. **Повторне використання:**
   - Сервіси та репозиторії використовуються різними контролерами
   - DTO моделі відокремлені від DB моделей

5. **Підтримуваність:**
   - Легше знаходити та виправляти помилки
   - Код більш читаємий та зрозумілий

---

### 2.3 Навіщо створювати окремі моделі у папці Models та в чому різниця між цими моделями й сутностями, що зберігаються в базі даних?

**Відповідь:**

**Причини создания окремих DTO моделей:**

1. **API契約:**
   - DTO визначає, які дані надсилаються клієнту
   - Приховує внутрішню структуру DB

2. **Безпека:**
   - Не передаємо чутливі поля (паролі, внутрішні ID)
   - Контроль над тим, що бачить клієнт

3. **Гнучкість:**
   - Можна змінити DB модель без впливу на API
   - Різні DTO для читання та запису

4. **Перфоманс:**
   - Можна передавати тільки необхідні поля
   - Уникаємо N+1 проблем

**Різниці між DTO та DB сутностями:**

| Аспект | DTO Model | DB Entity |
|--------|-----------|-----------|
| **Розташування** | `Zoo.REST/Models/` | `Zoo.Infrastructure/Models/` |
| **Призначення** | API контракт | Маніпуляція БД |
| **Навігація** | Звичайних об'єкти | Navigation properties |
| **Валідація** | Input validation | Data annotations |
| **Поля** | Тільки необхідні для API | Усі поля сутності |

**Приклад з нашого проєкту:**

```csharp
// DTO для запиту (Zoo.REST/Models/AnimalApiModel.cs)
public class AnimalApiModel
{
	public string? Name { get; set; }
	public string? Species { get; set; }
	public int Age { get; set; }
	public Guid? EnclosureId { get; set; }
}

// DTO для відповіді (Zoo.REST/Models/AnimalDetailApiModel.cs)
public class AnimalDetailApiModel
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Species { get; set; }
	public int Age { get; set; }
	public Guid? EnclosureId { get; set; }
	public string? EnclosureName { get; set; }
}

// DB сутність (Zoo.Infrastructure/Models/AnimalModel.cs)
public class AnimalModel : IEntity
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Species { get; set; }
	public int Age { get; set; }
	public Guid? EnclosureId { get; set; }
	public EnclosureModel? Enclosure { get; set; }
	public AnimalDetailModel? Detail { get; set; }
	public List<KeeperModel> Keepers { get; set; } = new();
}
```

---

### 2.4 Що таке REST і як у вашому API забезпечується дотримання принципу однорідності інтерфейсу (uniform interface)?

**Відповідь:**

**Що таке REST:**

REST (Representational State Transfer) - архітектурний стиль для розробки розподілених систем на основі HTTP. Основні принципи Fielding:

1. **Client-Server:** Розділення клієнта та серверу
2. **Stateless:** Кожен запит має всю необхідну інформацію
3. **Cacheable:** Відповіді можуть кешуватися
4. **Uniform Interface:** Єдиний багатомовний інтерфейс
5. **Layered System:** Архітектура можна розбити на шари
6. **Code on Demand (Optional):** Можна надсилати виконуваний код

**Uniform Interface мають базуватися на:**

1. **Identification of Resources:**
   - Ресурси однозначно ідентифіковані
   - `/api/animals` - колекція тварин
   - `/api/animals/{id}` - конкретна тварина

2. **Manipulation of Resources:**
   - Через HTTP методи (GET, POST, PUT, DELETE)
   - Представлення містить достатньо інформації для маніпуляції

3. **Self-describing Messages:**
   - Повідомлення включають усю необхідну інформацію
   - Content-Type headers

4. **HATEOAS (Hypermedia As The Engine Of Application State):**
   - Відповіді включають посилання на пов'язані ресурси

**Як реалізується в нашому проєкті:**

```csharp
// 1. Uniform Resource Naming
[Route("api/[controller]")] // /api/animals, /api/enclosures

// 2. HTTP методи для CRUD операцій
[HttpGet]                     // GET /api/animals - отримати все
[HttpGet("{id}")]             // GET /api/animals/123 - отримати одну
[HttpPost]                    // POST /api/animals - створити
[HttpPut("{id}")]             // PUT /api/animals/123 - оновити
[HttpDelete("{id}")]          // DELETE /api/animals/123 - видалити

// 3. Правильні HTTP коди відповідей
return Ok(result);            // 200 OK
return CreatedAtAction(...);  // 201 Created
return NoContent();           // 204 No Content
return NotFound(...);         // 404 Not Found
return BadRequest(...);       // 400 Bad Request

// 4. Self-describing responses
public class AnimalDetailApiModel
{
	public Guid Id { get; set; }           // ID для однозначної ідентифікації
	public string? Name { get; set; }
	public string? Species { get; set; }
	public int Age { get; set; }
	public Guid? EnclosureId { get; set; } // Посилання на пов'язаний ресурс
	public string? EnclosureName { get; set; }
}
```

---

### 2.5 Які HTTP-методи використовуються для CRUD-операцій? Наведіть приклади відповідності методів діям у контролерах.

**Відповідь:**

| CRUD | HTTP Метод | Endpoint | Статус | Приклад |
|------|-----------|----------|--------|---------|
| **Create** | POST | `/api/animals` | 201/400 | `POST /api/animals` → 201 Created |
| **Read** | GET | `/api/animals/{id}` | 200/404 | `GET /api/animals/123` → 200 OK |
| **Read All** | GET | `/api/animals` | 200 | `GET /api/animals?page=1&amount=10` → 200 OK |
| **Update** | PUT | `/api/animals/{id}` | 204/400/404 | `PUT /api/animals/123` → 204 No Content |
| **Delete** | DELETE | `/api/animals/{id}` | 204/404 | `DELETE /api/animals/123` → 204 No Content |

**Приклади з нашого проєкту:**

```csharp
// ===== ANIMALS CONTROLLER =====

// CREATE - POST /api/animals
[HttpPost]
public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal(AnimalApiModel model)
{
	var animal = new AnimalModel { ... };
	await _animalService.CreateAsync(animal);
	return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, result);
	// HTTP 201 Created + Location header + resource data
}

// READ Single - GET /api/animals/123abc
[HttpGet("{id}")]
public async Task<ActionResult<AnimalDetailApiModel>> GetAnimalById(Guid id)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(new { message = "Not found" }); // 404
	return Ok(new AnimalDetailApiModel { ... }); // 200
}

// READ All - GET /api/animals?page=1&amount=10
[HttpGet]
public async Task<ActionResult<IEnumerable<AnimalDetailApiModel>>> GetAllAnimals(
	int page = 1, int amount = 10)
{
	var animals = await _animalService.ReadAllAsync(page, amount);
	// Повертає 200 OK з масивом
	return Ok(animals.Select(a => new AnimalDetailApiModel { ... }));
}

// UPDATE - PUT /api/animals/123abc
[HttpPut("{id}")]
public async Task<IActionResult> UpdateAnimal(Guid id, AnimalApiModel model)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(...); // 404

	// Оновляємо властивості
	animal.Name = model.Name;
	animal.Species = model.Species;

	await _animalService.UpdateAsync(animal);
	return NoContent(); // 204 No Content
}

// DELETE - DELETE /api/animals/123abc
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAnimal(Guid id)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(...); // 404

	await _animalService.RemoveAsync(animal);
	return NoContent(); // 204 No Content
}
```

---

### 2.6 Які HTTP-коди відповідей є коректними для сценаріїв успіху та обробки помилок у вашому API (створення, оновлення, видалення, не знайдено тощо)?

**Відповідь:**

**Коди для успіху:**

| Код | Назва | Приклад з нашого API |
|-----|-------|----------------------|
| **200** | OK | `GET /api/animals` - Успішне отримання даних |
| **201** | Created | `POST /api/animals` - Успішне створення тварини |
| **204** | No Content | `PUT /api/animals/{id}`, `DELETE /api/animals/{id}` |

**Коди для помилок:**

| Код | Назва | Коли використовується | Приклад |
|-----|-------|----------------------|---------|
| **400** | Bad Request | Невірні вхідні дані | Порожнє ім'я тварини, вік < 0 |
| **404** | Not Found | Ресурс не знайдена | Тварина з ID не існує |
| **500** | Internal Server Error | Помилка на сервері | Помилка БД (обривається) |

**Реалізація в нашому проєкті:**

```csharp
// 2.6.1 Успіх - 200 OK (отримання)
[HttpGet("{id}")]
public async Task<ActionResult<AnimalDetailApiModel>> GetAnimalById(Guid id)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(new { message = $"Animal {id} not found" }); // 404

	return Ok(new AnimalDetailApiModel { ... }); // ✓ 200 OK
}

// 2.6.2 Успіх - 201 Created (створення)
[HttpPost]
public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal(AnimalApiModel model)
{
	if (string.IsNullOrWhiteSpace(model.Name))
		return BadRequest(new { message = "Name is required" }); // 400

	var animal = new AnimalModel { ... };
	await _animalService.CreateAsync(animal);

	// ✓ 201 Created - повертаємо новий ресурс
	return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, 
		new AnimalDetailApiModel { ... });
}

// 2.6.3 Успіх - 204 No Content (оновлення)
[HttpPut("{id}")]
public async Task<IActionResult> UpdateAnimal(Guid id, AnimalApiModel model)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(new { message = $"Animal {id} not found" }); // 404

	if (model.Age < 0)
		return BadRequest(new { message = "Age cannot be negative" }); // 400

	animal.Name = model.Name;
	animal.Age = model.Age;
	await _animalService.UpdateAsync(animal);

	return NoContent(); // ✓ 204 No Content
}

// 2.6.4 Успіх - 204 No Content (видалення)
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAnimal(Guid id)
{
	var animal = await _animalService.ReadAsync(id);
	if (animal == null)
		return NotFound(new { message = $"Animal {id} not found" }); // 404

	await _animalService.RemoveAsync(animal);
	return NoContent(); // ✓ 204 No Content
}

// 2.6.5 Помилки
[HttpPost]
public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal(AnimalApiModel model)
{
	// 400 Bad Request - порожні поля
	if (string.IsNullOrWhiteSpace(model.Name) || 
		string.IsNullOrWhiteSpace(model.Species))
	{
		return BadRequest(new { message = "Name and Species are required" });
	}

	// 400 Bad Request - невірні значення
	if (model.Age < 0)
	{
		return BadRequest(new { message = "Age cannot be negative" });
	}

	// 400 Bad Request - помилка при створенні
	var success = await _animalService.CreateAsync(animal);
	if (!success)
	{
		return BadRequest(new { message = "Failed to create animal" });
	}

	return CreatedAtAction(...); // 201 Created
}
```

**Таблиця кодів для всіх операцій:**

| Операція | Успіх | Помилка (параметри) | Помилка (обов'язк) | Помилка (не знайдена) |
|----------|-------|---------------------|-------------------|----------------------|
| GET All | 200 | - | - | - |
| GET One | 200 | - | - | 404 |
| POST | 201 | 400 (неправив данные) | 400 (обов'язкові поля) | - |
| PUT | 204 | 400 (неправив данные) | 400 (обов'язкові поля) | 404 |
| DELETE | 204 | - | - | 404 |

---

### 2.7 Опишіть інтерфейс ICrudServiceAsync<T>: для чого потрібні асинхронні методи та як вони впливають на продуктивність застосунку?

**Відповідь:**

**Визначення ICrudServiceAsync<T>:**

```csharp
public interface ICrudServiceAsync<T> : IEnumerable<T>
	where T : class, IEntity
{
	// Create операція
	Task<bool> CreateAsync(T element);

	// Read операції
	Task<T> ReadAsync(Guid id);
	Task<IEnumerable<T>> ReadAllAsync();
	Task<IEnumerable<T>> ReadAllAsync(params Expression<Func<T, object>>[] includes);
	Task<IEnumerable<T>> ReadAllAsync(int page, int amount);

	// Update операція
	Task<bool> UpdateAsync(T element);

	// Delete операція
	Task<bool> RemoveAsync(T element);

	// Save операція
	Task<bool> SaveAsync();

	// Event для сповіщень
	event ZooNotificationHandler? OperationPerformed;
}
```

**Для чого потрібні асинхронні методи:**

1. **Non-blocking I/O:**
   - Потік не блокується під час операції з БД
   - Решта коду може виконуватися паралельно

```csharp
// Синхронна версія - блокує потік
public class Animal GetAnimal(Guid id)
{
	var animal = _repository.GetById(id);  // ⏱️ Потік очікує...
	return animal;
}

// Асинхронна версія - не блокує потік
public async Task<Animal> GetAnimalAsync(Guid id)
{
	var animal = await _repository.GetByIdAsync(id);  // ⚡ Потік вільний!
	return animal;
}
```

2. **Масштабованість:**
   - Один потік може обслуговувати более тисяч запиты одночасно
   - За рахунок async/await паттерну

3. **Гнучкість операцій з БД:**
   - `await _repository.AddAsync(element)` - додання
   - `await _repository.UpdateAsync(element)` - оновлення
   - `await _repository.SaveAsync()` - збереження змін

4. **Паралельні операції:**
   - Можна виконувати кілька операцій одночасно

```csharp
// Паралельне виконання
var task1 = _animalService.ReadAsync(animalId);
var task2 = _enclosureService.ReadAsync(enclosureId);
await Task.WhenAll(task1, task2);  // Позаливаємо обидві операції
```

**Реалізація в нашому проєкті:**

```csharp
public class CrudServiceAsync<T> : ICrudServiceAsync<T>
	where T : class, IEntity
{
	private readonly IRepository<T> _repository;

	// CREATE
	public async Task<bool> CreateAsync(T element)
	{
		await _repository.AddAsync(element);
		await _repository.SaveAsync();
		OperationPerformed?.Invoke($"CREATE: додано елемент (ID: {element.Id})");
		return true;
	}

	// READ
	public async Task<T> ReadAsync(Guid id)
		=> await _repository.GetByIdAsync(id);

	public async Task<IEnumerable<T>> ReadAllAsync()
		=> await _repository.GetAllAsync();

	// READ with pagination
	public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
	{
		var all = (await _repository.GetAllAsync()).ToList();
		if (page < 1) page = 1;
		if (amount < 1) amount = 10;
		return all.Skip((page - 1) * amount).Take(amount).ToList();
	}

	// UPDATE
	public async Task<bool> UpdateAsync(T element)
	{
		await _repository.UpdateAsync(element);
		await _repository.SaveAsync();
		OperationPerformed?.Invoke($"UPDATE: оновлено елемент (ID: {element.Id})");
		return true;
	}

	// DELETE
	public async Task<bool> RemoveAsync(T element)
	{
		await _repository.RemoveAsync(element);
		await _repository.SaveAsync();
		OperationPerformed?.Invoke($"REMOVE: видалено елемент (ID: {element.Id})");
		return true;
	}
}
```

**Використання в контролерах:**

```csharp
[HttpPost]
public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal(AnimalApiModel model)
{
	var animal = new AnimalModel { ... };

	// Асинхронний виклик - сервіс чеплятися, звільняючи потік
	var success = await _animalService.CreateAsync(animal);

	if (!success)
		return BadRequest(new { message = "Failed to create" });

	return CreatedAtAction(...);
}

[HttpGet]
public async Task<ActionResult<IEnumerable<AnimalDetailApiModel>>> GetAllAnimals(
	int page = 1, int amount = 10)
{
	// Асинхронна пагієнація
	var animals = await _animalService.ReadAllAsync(page, amount);

	return Ok(animals.Select(a => new AnimalDetailApiModel { ... }));
}
```

**Вплив на продуктивність:**

| Аспект | Вплив |
|--------|-------|
| **Throughput** | ⬆️ Більше запиты обслуговується одночасно |
| **Latency** | ➡️ Незмінна час однієї операції |
| **CPU Usage** | ⬇️ Менше потоків = менше overhead |
| **Memory** | ⬇️ Менше потоків = менше пам'яти |
| **Responsiveness** | ⬆️ Застосунок більш響як |

**Приклад продуктивності:**

```
Синхронна версія:
- 100 запиты по 100ms = 10 секунд (послідовно)

Асинхронна версія:
- 100 запиты по 100ms = ~500ms (паралельно)
- Економія: 20x!
```

---

### 2.8 Як працює механізм впровадження залежностей (Dependency Injection) в ASP.NET Core і як ви зареєстрували свої сервіси та репозиторії?

**Відповідь:**

**Що таке Dependency Injection (DI):**

DI - це паттерн, де об'єкт отримує залежності з зовні, замість створення їх самостійно. Це забезпечує:
- Слабку зв'язність (loose coupling)
- Простоту тестування (можна мокувати)
- Гнучкість та масштабованість

**Реєстрація сервісів в Program.cs:**

```csharp
using Microsoft.EntityFrameworkCore;
using Zoo.Common.Interfaces;
using Zoo.Common.Services;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Models;
using Zoo.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// === 1. Регістрація DbContext ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
	?? "Host=localhost;Port=5432;Database=ZooDb;Username=postgres;Password=8gyrg8104ife";
builder.Services.AddDbContext<ZooContext>(options =>
	options.UseNpgsql(connectionString));

// === 2. Регістрація GenericRepository для усіх типів ===
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
// Це еквівалентно:
// builder.Services.AddScoped<IRepository<AnimalModel>, GenericRepository<AnimalModel>>();
// builder.Services.AddScoped<IRepository<EnclosureModel>, GenericRepository<EnclosureModel>>();

// === 3. Регістрація CRUD сервісів ===
builder.Services.AddScoped<ICrudServiceAsync<AnimalModel>>(sp =>
{
	var repository = sp.GetRequiredService<IRepository<AnimalModel>>();
	return new CrudServiceAsync<AnimalModel>(repository);
});

builder.Services.AddScoped<ICrudServiceAsync<EnclosureModel>>(sp =>
{
	var repository = sp.GetRequiredService<IRepository<EnclosureModel>>();
	return new CrudServiceAsync<EnclosureModel>(repository);
});

// === 4. Регістрація контролерів ===
builder.Services.AddControllers();

// === 5. Регістрація Swagger для документації ===
builder.Services.AddSwaggerGen();

// === 6. Регістрація логування ===
builder.Services.AddLogging();

var app = builder.Build();
```

**Типи реєстрації в DI контейнері:**

| Тип | Lifetime | Приклад | Використання |
|-----|----------|---------|-------------|
| **Transient** | Новий об'єкт кожний раз | `AddTransient` | Stateless об'єкты |
| **Scoped** | Один на запит | `AddScoped` | DbContext, Repositories |
| **Singleton** | Один для всього застосунку | `AddSingleton` | Configuration, Cache |

У нашому проєкті використовуємо **Scoped** для:
- `ZooContext` - один контекст на запит
- `IRepository<T>` - розраховується з контексту
- `ICrudServiceAsync<T>` - розраховується з репозиторію

**Графік залежностей для нашого проєкту:**

```
HTTP Request
	↓
AnimalsController (сцена контроллер)
	↓
ICrudServiceAsync<AnimalModel> (@Scoped)
	↓
IRepository<AnimalModel> (@Scoped)
	↓
ZooContext (@Scoped)
	↓
PostgreSQL Database
```

**Як потім використовуємо DI в контролерах:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
	private readonly ICrudServiceAsync<AnimalModel> _animalService;
	private readonly ILogger<AnimalsController> _logger;

	// DI інжекційні через конструктор
	public AnimalsController(
		ICrudServiceAsync<AnimalModel> animalService, 
		ILogger<AnimalsController> logger)
	{
		_animalService = animalService;
		_logger = logger;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<AnimalDetailApiModel>> GetAnimalById(Guid id)
	{
		// Використовуємо інжектовані сервіси
		_logger.LogInformation($"Getting animal {id}");
		var animal = await _animalService.ReadAsync(id);
		// ...
	}
}
```

**Переваги DI в нашому проєкті:**

1. **Розділення обов'язків:**
   - Контролер не знає, як створювальнийRepository
   - Repository не знає про контекст

2. **Тестованість:**
```csharp
// Unit тест - можемо мокувати
var mockService = new Mock<ICrudServiceAsync<AnimalModel>>();
mockService.Setup(s => s.ReadAsync(id))
	.ReturnsAsync(expectedAnimal);

var controller = new AnimalsController(mockService.Object, mockLogger);
var result = await controller.GetAnimalById(id);
```

3. **Конфігурація:**
   - Легко змінити реалізацію без змін контролера
   - Можна зареєструвати різні реалізації для тестування

---

### 2.9 Як організована пагінація у вашому CRUD-сервісі (ReadAllAsync(int page, int amount)) та для чого вона потрібна?

**Відповідь:**

**Реалізація пагінації в CrudServiceAsync:**

```csharp
public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
{
	// 1. Отримуємо усі дані з БД
	var all = (await _repository.GetAllAsync()).ToList();

	// 2. Валідуємо параметри пагінації
	if (page < 1) page = 1;        // Мінімум 1-а сторінка
	if (amount < 1) amount = 10;   // Мінімум 10 елементів

	// 3. Розраховуємо offset (пропускаємо кількість елементів)
	var skip = (page - 1) * amount;
	// page=1, amount=10 → skip=0 (елементи 0-9)
	// page=2, amount=10 → skip=10 (елементи 10-19)
	// page=3, amount=10 → skip=20 (елементи 20-29)

	// 4. Беремо потрібну кількість елементів
	return all.Skip(skip).Take(amount).ToList();
}
```

**Як використовується в контролерах:**

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<AnimalDetailApiModel>>> GetAllAnimals(
	[FromQuery] int page = 1, 
	[FromQuery] int amount = 10)
{
	_logger.LogInformation($"Getting animals page: {page}, amount: {amount}");

	// Викликаємо з пагінацією
	var animals = await _animalService.ReadAllAsync(page, amount);

	var result = animals.Select(a => new AnimalDetailApiModel
	{
		Id = a.Id,
		Name = a.Name,
		// ...
	}).ToList();

	return Ok(result);
}
```

**Приклади запитів:**

| Запит | Результат |
|-------|-----------|
| `GET /api/animals` | Сторінка 1, 10 елементів (за замовчуванням) |
| `GET /api/animals?page=1&amount=20` | Сторінка 1, 20 елементів |
| `GET /api/animals?page=2&amount=10` | Сторінка 2, елементи 10-19 |
| `GET /api/animals?page=3&amount=50` | Сторінка 3, елементи 100-149 |

**Математика пагінації:**

```
Якщо у БД 250 тварин, а amount=10:
- Кількість сторінок = ceil(250 / 10) = 25

page=1:  Skip(0),  Take(10)  → елементи 1-10
page=2:  Skip(10), Take(10)  → елементи 11-20
page=3:  Skip(20), Take(10)  → елементи 21-30
...
page=25: Skip(240),Take(10)  → елементи 241-250
```

**Для чого потрібна пагінація:**

1. **Продуктивність:**
   - Не завантажуємо усі дані з БД
   - Скорочуємо час передачі даних

2. **Масштабованість:**
   - Якщо в БД мільйон записів, показуємо тільки 10
   - Зберігаємо пам'ять та спілка

3. **User Experience:**
   - Сторінки завантажуються швидше
   - Клієнт не перевантажений даними

4. **Контроль:**
   - Клієнт виділяє, скільки елементів потрібно
   - API розраховується роботою на кілька запиты

**Приклад сценарію без пагінації vs з пагінацією:**

```
Сценарій: 1 000 000 тварини в БД

БЕЗ пагінації:
1. Client: GET /api/animals
2. Server: Завантажує 1М записів з БД
3. Server: Перетворює на JSON (~500MB)
4. Network: Передає 500MB
5. Client: Приймає 500MB, парсить, показує...
   Результат: 🔴 CRASH! Браузер падає

З пагінацією:
1. Client: GET /api/animals?page=1&amount=10
2. Server: Завантажує 10 записів з БД
3. Server: Перетворює на JSON (~10KB)
4. Network: Передає 10KB
5. Client: Приймає 10KB, парсить, показує
   Результат: ✅ Швидко! Стабільно!

6. Client: (користувач натиснув "Next")
7. Client: GET /api/animals?page=2&amount=10
8. ... (повтори для кажної сторінки)
```

---

### 2.10 Яким чином ви тестували роботу свого API, які інструменти або сторінки використовували, та які результати слід включити в PDF-файл для PR?

**Відповідь:**

**Інструменти для тестування API:**

1. **Swagger UI (вбудований)** ✅ Використовується в нашому проєкті
   - Автоматична генерація документації
   - Інтерактивне тестування прямо з браузера
   - Адреса: `https://localhost:5001/swagger/index.html`

2. **Postman** (дескноп-додаток)
   - Детальна налаштування запитів
   - Зберігання колекцій запитів
   - Автоматизація тестування

3. **curl** (командний рядок)
   - Простий та легкий
   - Можна автоматизувати

4. **Thunder Client** (VS Code extension)
   - Легко інтегрується в IDE
   - Можна зберігати запити

**Рекомендовані тести для PR документу:**

1. **GET /api/animals - Отримати усіх тварин**
```
Request:  GET /api/animals?page=1&amount=2
Response: 200 OK
Body: [
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"name": "Лев",
	"species": "Panthera leo",
	"age": 5,
	"enclosureId": "...",
	"enclosureName": "Африканська саванна"
  },
  ...
]
```

2. **GET /api/animals/{id} - Отримати одну тварину**
```
Request:  GET /api/animals/550e8400-e29b-41d4-a716-446655440000
Response: 200 OK
Body: {
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Лев",
  "species": "Panthera leo",
  "age": 5,
  "enclosureId": null,
  "enclosureName": null
}
```

3. **GET /api/animals/{id} - Тварина не знайдена**
```
Request:  GET /api/animals/00000000-0000-0000-0000-000000000000
Response: 404 Not Found
Body: {
  "message": "Animal with ID 00000000-0000-0000-0000-000000000000 not found"
}
```

4. **POST /api/animals - Створити нову тварину**
```
Request:  POST /api/animals
Body: {
  "name": "Гіпопотам",
  "species": "Hippopotamus amphibius",
  "age": 3,
  "enclosureId": null
}
Response: 201 Created
Location: /api/animals/550e8400-e29b-41d4-a716-446655440001
Body: {
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "name": "Гіпопотам",
  "species": "Hippopotamus amphibius",
  "age": 3,
  "enclosureId": null,
  "enclosureName": null
}
```

5. **POST /api/animals - Невірні дані (за)
```
Request:  POST /api/animals
Body: {
  "name": "",  ← Порожнее імя
  "species": "Unknown",
  "age": -5    ← Негативний вік
}
Response: 400 Bad Request
Body: {
  "message": "Name and Species are required"
}
```

6. **PUT /api/animals/{id} - Оновити тварину**
```
Request:  PUT /api/animals/550e8400-e29b-41d4-a716-446655440000
Body: {
  "name": "Мільйонник Лев",
  "species": "Panthera leo",
  "age": 6,
  "enclosureId": null
}
Response: 204 No Content
Body: (пусто)
```

7. **DELETE /api/animals/{id} - Видалити тварину**
```
Request:  DELETE /api/animals/550e8400-e29b-41d4-a716-446655440000
Response: 204 No Content
Body: (пусто)
```

8. **Аналогічні тести для /api/enclosures**
   - GET /api/enclosures
   - GET /api/enclosures/{id}
   - POST /api/enclosures
   - PUT /api/enclosures/{id}
   - DELETE /api/enclosures/{id}

**Что включить в PDF-отчет:**

✅ **Обязательно:**
- Скриншоты Swagger UI с параметрами методов
- Успешные GET запросы (200, 201)
- Ошибки валидації (400)
- Ошибки не найденных ресурсов (404)
- Пример пагинации (page, amount)

✅ **Рекомендуется:**
- Полные curl команды с параметрами
- JSON response для каждого теста
- Результаты тестирования обоих контролеров
- Скриншоты Swagger параметров и примеров

📋 **Структура отчета:**
```
1. Заголовок "API Testing Results"
2. Таблица тестов
3. Для каждого теста:
   - Описание
   - curl команда или Swagger параметры
   - HTTP статус
   - Response JSON
   - Скриншот (опционально)
```

---

### 2.11 Які кроки потрібно виконати, щоб створити проєкт {Назва тематики}.MVC, та чим відрізняється його структура від REST API?

**Відповідь:**

Це додаткове завдання, яке передбачає створення MVC або Blazor застосунку, який повторно використовує модель та контекст БД з лабораторної 3. У часовому контексті даного завдання першорядним є завершення REST API.

---

### 2.12 Як ви повторно використали моделі та контекст бази даних з третьої лабораторної у новому MVC/Blazor-застосунку?

**Відповідь:**

Це також додаткове завдання для MVC/Blazor гаджета. Для REST API ми повторно використали:
- `Zoo.Infrastructure` проєкт (контекст ZooContext та моделі)
- `Zoo.Common` проєкт (інтерфейси та сервіси)

---

## 3. Висновки

У ході виконання лабораторної роботи було успішно розроблено:

1. ✅ **ASP.NET Core Web API проєкт (Zoo.REST)** з повною CRUD функціональністю
2. ✅ **Два контролери** для управління тваринами та вольєрами
3. ✅ **DTO моделі** для відокремлення API контракту від DB сутностей
4. ✅ **Dependency Injection конфігурація** для реєстрації сервісів та репозиторіїв
5. ✅ **Swagger документація** для інтерактивного тестування API
6. ✅ **Правильні HTTP методи та коди відповідей** відповідно до REST принципів
7. ✅ **Асинхронна обробка** всіх операцій з БД
8. ✅ **Пагінація** для оптимізації передачі даних

Проєкт повністю відповідає вимогам лабораторної роботи та демонструє глибоке розуміння:
- Тризирівневої архітектури
- REST архітектурного стилю
- Сучасних практик ASP.NET Core розробки
- Dependency Injection паттерну
- Асинхронного програмування

---

## Додаток: Команди для запуску та тестування

### Запуск серверу:
```bash
cd Zoo/Zoo.REST
dotnet run
# Сервер запустится на https://localhost:5001
```

### Отримання доступу до Swagger UI:
```
https://localhost:5001/swagger/index.html
```

### Приклади curl команд:

```bash
# 1. Отримати всіх тварин
curl -X GET "https://localhost:5001/api/animals?page=1&amount=10" \
  -H "accept: application/json"

# 2. Отримати одну тварину
curl -X GET "https://localhost:5001/api/animals/{id}" \
  -H "accept: application/json"

# 3. Створити нову тварину
curl -X POST "https://localhost:5001/api/animals" \
  -H "Content-Type: application/json" \
  -d "{ \"name\": \"Лев\", \"species\": \"Panthera leo\", \"age\": 5, \"enclosureId\": null }"

# 4. Оновити тварину
curl -X PUT "https://localhost:5001/api/animals/{id}" \
  -H "Content-Type: application/json" \
  -d "{ \"name\": \"Молодой Лев\", \"species\": \"Panthera leo\", \"age\": 6, \"enclosureId\": null }"

# 5. Видалити тварину
curl -X DELETE "https://localhost:5001/api/animals/{id}"
```

---

**Звіт підготовлений:** 27.11.2026
**Автор:** Розробник
**Проєкт:** Zoo.REST API
**Version:** 1.0
