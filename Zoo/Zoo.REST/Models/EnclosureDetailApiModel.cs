namespace Zoo.REST.Models;

/// <summary>
/// DTO model for detailed enclosure information in API responses.
/// Includes enclosure properties and list of animals in the enclosure.
/// </summary>
public class EnclosureDetailApiModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double Area { get; set; }
    public int Capacity { get; set; }
    public int AnimalCount { get; set; }
    public List<string> AnimalNames { get; set; } = new();
}
