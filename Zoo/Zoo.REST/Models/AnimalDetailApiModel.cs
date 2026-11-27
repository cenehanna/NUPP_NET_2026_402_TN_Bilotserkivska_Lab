namespace Zoo.REST.Models;

/// <summary>
/// DTO model for detailed animal information in API responses.
/// Includes all animal properties plus enclosure information.
/// </summary>
public class AnimalDetailApiModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int Age { get; set; }
    public Guid? EnclosureId { get; set; }
    public string? EnclosureName { get; set; }
}
