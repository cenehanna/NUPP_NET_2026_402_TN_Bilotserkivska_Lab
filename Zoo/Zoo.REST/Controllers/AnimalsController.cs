using Microsoft.AspNetCore.Mvc;
using Zoo.Common.Interfaces;
using Zoo.Infrastructure.Models;
using Zoo.REST.Models;

namespace Zoo.REST.Controllers;

/// <summary>
/// REST API controller for managing Zoo animals.
/// Implements CRUD operations following REST principles with proper HTTP methods and status codes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnimalsController : ControllerBase
{
    private readonly ICrudServiceAsync<AnimalModel> _animalService;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(ICrudServiceAsync<AnimalModel> animalService, ILogger<AnimalsController> logger)
    {
        _animalService = animalService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all animals with optional pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="amount">Items per page (default: 10)</param>
    /// <returns>List of animals - 200 OK</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimalDetailApiModel>>> GetAllAnimals([FromQuery] int page = 1, [FromQuery] int amount = 10)
    {
        _logger.LogInformation($"Getting animals page: {page}, amount: {amount}");

        var animals = await _animalService.ReadAllAsync(page, amount);
        var result = animals.Select(a => new AnimalDetailApiModel
        {
            Id = a.Id,
            Name = a.Name,
            Species = a.Species,
            Age = a.Age,
            EnclosureId = a.EnclosureId,
            EnclosureName = a.Enclosure?.Name
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Gets a specific animal by ID.
    /// </summary>
    /// <param name="id">Animal ID</param>
    /// <returns>Animal details - 200 OK or 404 Not Found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AnimalDetailApiModel>> GetAnimalById(Guid id)
    {
        _logger.LogInformation($"Getting animal with ID: {id}");

        var animal = await _animalService.ReadAsync(id);
        if (animal == null)
        {
            _logger.LogWarning($"Animal not found with ID: {id}");
            return NotFound(new { message = $"Animal with ID {id} not found" });
        }

        var result = new AnimalDetailApiModel
        {
            Id = animal.Id,
            Name = animal.Name,
            Species = animal.Species,
            Age = animal.Age,
            EnclosureId = animal.EnclosureId,
            EnclosureName = animal.Enclosure?.Name
        };

        return Ok(result);
    }

    /// <summary>
    /// Creates a new animal.
    /// </summary>
    /// <param name="model">Animal data</param>
    /// <returns>Created animal with ID - 201 Created or 400 Bad Request</returns>
    [HttpPost]
    public async Task<ActionResult<AnimalDetailApiModel>> CreateAnimal([FromBody] AnimalApiModel model)
    {
        _logger.LogInformation($"Creating new animal: {model.Name}");

        if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Species))
        {
            return BadRequest(new { message = "Name and Species are required" });
        }

        if (model.Age < 0)
        {
            return BadRequest(new { message = "Age cannot be negative" });
        }

        var animal = new AnimalModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Species = model.Species,
            Age = model.Age,
            EnclosureId = model.EnclosureId
        };

        var success = await _animalService.CreateAsync(animal);
        if (!success)
        {
            _logger.LogError($"Failed to create animal");
            return BadRequest(new { message = "Failed to create animal" });
        }

        var result = new AnimalDetailApiModel
        {
            Id = animal.Id,
            Name = animal.Name,
            Species = animal.Species,
            Age = animal.Age,
            EnclosureId = animal.EnclosureId
        };

        return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, result);
    }

    /// <summary>
    /// Updates an existing animal.
    /// </summary>
    /// <param name="id">Animal ID</param>
    /// <param name="model">Updated animal data</param>
    /// <returns>204 No Content, 404 Not Found, or 400 Bad Request</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnimal(Guid id, [FromBody] AnimalApiModel model)
    {
        _logger.LogInformation($"Updating animal with ID: {id}");

        var animal = await _animalService.ReadAsync(id);
        if (animal == null)
        {
            _logger.LogWarning($"Animal not found with ID: {id}");
            return NotFound(new { message = $"Animal with ID {id} not found" });
        }

        if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Species))
        {
            return BadRequest(new { message = "Name and Species are required" });
        }

        if (model.Age < 0)
        {
            return BadRequest(new { message = "Age cannot be negative" });
        }

        animal.Name = model.Name;
        animal.Species = model.Species;
        animal.Age = model.Age;
        animal.EnclosureId = model.EnclosureId;

        var success = await _animalService.UpdateAsync(animal);
        if (!success)
        {
            _logger.LogError($"Failed to update animal with ID: {id}");
            return BadRequest(new { message = "Failed to update animal" });
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an animal.
    /// </summary>
    /// <param name="id">Animal ID</param>
    /// <returns>204 No Content or 404 Not Found</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(Guid id)
    {
        _logger.LogInformation($"Deleting animal with ID: {id}");

        var animal = await _animalService.ReadAsync(id);
        if (animal == null)
        {
            _logger.LogWarning($"Animal not found with ID: {id}");
            return NotFound(new { message = $"Animal with ID {id} not found" });
        }

        var success = await _animalService.RemoveAsync(animal);
        if (!success)
        {
            _logger.LogError($"Failed to delete animal with ID: {id}");
            return BadRequest(new { message = "Failed to delete animal" });
        }

        return NoContent();
    }
}
