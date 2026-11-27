namespace Zoo.REST.Models;

/// <summary>
/// DTO model for creating or updating an animal.
/// Contains only the necessary properties for API operations.
/// </summary>
public class AnimalApiModel
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int Age { get; set; }
    public Guid? EnclosureId { get; set; }
}
