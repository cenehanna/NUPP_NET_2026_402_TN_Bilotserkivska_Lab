namespace Zoo.Common.Interfaces;

// Інтерфейс сутності з обов'язковим Id та необов'язковим іменем
public interface IEntity
{
    Guid Id { get; set; }
    string? Name { get; set; }
}
