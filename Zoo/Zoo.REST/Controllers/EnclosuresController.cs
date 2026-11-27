using Microsoft.AspNetCore.Mvc;
using Zoo.Common.Interfaces;
using Zoo.Infrastructure.Models;
using Zoo.REST.Models;

namespace Zoo.REST.Controllers;

/// <summary>
/// REST API controller for managing Zoo enclosures (habitats).
/// Implements CRUD operations following REST principles with proper HTTP methods and status codes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnclosuresController : ControllerBase
{
    private readonly ICrudServiceAsync<EnclosureModel> _enclosureService;
    private readonly ILogger<EnclosuresController> _logger;

    public EnclosuresController(ICrudServiceAsync<EnclosureModel> enclosureService, ILogger<EnclosuresController> logger)
    {
        _enclosureService = enclosureService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all enclosures with optional pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="amount">Items per page (default: 10)</param>
    /// <returns>List of enclosures - 200 OK</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnclosureDetailApiModel>>> GetAllEnclosures([FromQuery] int page = 1, [FromQuery] int amount = 10)
    {
        _logger.LogInformation($"Getting enclosures page: {page}, amount: {amount}");

        var enclosures = await _enclosureService.ReadAllAsync(page, amount);
        var result = enclosures.Select(e => new EnclosureDetailApiModel
        {
            Id = e.Id,
            Name = e.Name,
            Area = e.Area,
            Capacity = e.Capacity,
            AnimalCount = e.Animals?.Count ?? 0,
            AnimalNames = e.Animals?.Select(a => a.Name ?? "Unknown").ToList() ?? new List<string>()
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Gets a specific enclosure by ID.
    /// </summary>
    /// <param name="id">Enclosure ID</param>
    /// <returns>Enclosure details - 200 OK or 404 Not Found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnclosureDetailApiModel>> GetEnclosureById(Guid id)
    {
        _logger.LogInformation($"Getting enclosure with ID: {id}");

        var enclosure = await _enclosureService.ReadAsync(id);
        if (enclosure == null)
        {
            _logger.LogWarning($"Enclosure not found with ID: {id}");
            return NotFound(new { message = $"Enclosure with ID {id} not found" });
        }

        var result = new EnclosureDetailApiModel
        {
            Id = enclosure.Id,
            Name = enclosure.Name,
            Area = enclosure.Area,
            Capacity = enclosure.Capacity,
            AnimalCount = enclosure.Animals?.Count ?? 0,
            AnimalNames = enclosure.Animals?.Select(a => a.Name ?? "Unknown").ToList() ?? new List<string>()
        };

        return Ok(result);
    }

    /// <summary>
    /// Creates a new enclosure.
    /// </summary>
    /// <param name="model">Enclosure data</param>
    /// <returns>Created enclosure with ID - 201 Created or 400 Bad Request</returns>
    [HttpPost]
    public async Task<ActionResult<EnclosureDetailApiModel>> CreateEnclosure([FromBody] EnclosureApiModel model)
    {
        _logger.LogInformation($"Creating new enclosure: {model.Name}");

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return BadRequest(new { message = "Enclosure name is required" });
        }

        if (model.Area <= 0)
        {
            return BadRequest(new { message = "Area must be greater than 0" });
        }

        if (model.Capacity <= 0)
        {
            return BadRequest(new { message = "Capacity must be greater than 0" });
        }

        var enclosure = new EnclosureModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Area = model.Area,
            Capacity = model.Capacity,
            Animals = new List<AnimalModel>()
        };

        var success = await _enclosureService.CreateAsync(enclosure);
        if (!success)
        {
            _logger.LogError($"Failed to create enclosure");
            return BadRequest(new { message = "Failed to create enclosure" });
        }

        var result = new EnclosureDetailApiModel
        {
            Id = enclosure.Id,
            Name = enclosure.Name,
            Area = enclosure.Area,
            Capacity = enclosure.Capacity,
            AnimalCount = 0,
            AnimalNames = new List<string>()
        };

        return CreatedAtAction(nameof(GetEnclosureById), new { id = enclosure.Id }, result);
    }

    /// <summary>
    /// Updates an existing enclosure.
    /// </summary>
    /// <param name="id">Enclosure ID</param>
    /// <param name="model">Updated enclosure data</param>
    /// <returns>204 No Content, 404 Not Found, or 400 Bad Request</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnclosure(Guid id, [FromBody] EnclosureApiModel model)
    {
        _logger.LogInformation($"Updating enclosure with ID: {id}");

        var enclosure = await _enclosureService.ReadAsync(id);
        if (enclosure == null)
        {
            _logger.LogWarning($"Enclosure not found with ID: {id}");
            return NotFound(new { message = $"Enclosure with ID {id} not found" });
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return BadRequest(new { message = "Enclosure name is required" });
        }

        if (model.Area <= 0)
        {
            return BadRequest(new { message = "Area must be greater than 0" });
        }

        if (model.Capacity <= 0)
        {
            return BadRequest(new { message = "Capacity must be greater than 0" });
        }

        enclosure.Name = model.Name;
        enclosure.Area = model.Area;
        enclosure.Capacity = model.Capacity;

        var success = await _enclosureService.UpdateAsync(enclosure);
        if (!success)
        {
            _logger.LogError($"Failed to update enclosure with ID: {id}");
            return BadRequest(new { message = "Failed to update enclosure" });
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an enclosure.
    /// </summary>
    /// <param name="id">Enclosure ID</param>
    /// <returns>204 No Content or 404 Not Found</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnclosure(Guid id)
    {
        _logger.LogInformation($"Deleting enclosure with ID: {id}");

        var enclosure = await _enclosureService.ReadAsync(id);
        if (enclosure == null)
        {
            _logger.LogWarning($"Enclosure not found with ID: {id}");
            return NotFound(new { message = $"Enclosure with ID {id} not found" });
        }

        var success = await _enclosureService.RemoveAsync(enclosure);
        if (!success)
        {
            _logger.LogError($"Failed to delete enclosure with ID: {id}");
            return BadRequest(new { message = "Failed to delete enclosure" });
        }

        return NoContent();
    }
}
