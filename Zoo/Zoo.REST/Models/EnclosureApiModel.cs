namespace Zoo.REST.Models;

/// <summary>
/// DTO model for creating or updating an enclosure.
/// Contains essential properties for enclosure management.
/// </summary>
public class EnclosureApiModel
{
    public string? Name { get; set; }
    public double Area { get; set; }
    public int Capacity { get; set; }
}
