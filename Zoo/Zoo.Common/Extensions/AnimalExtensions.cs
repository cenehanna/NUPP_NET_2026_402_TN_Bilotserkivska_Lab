namespace Zoo.Common;

public static class AnimalExtensions
{
    // Метод розширення
    public static string GetShortInfo(this Animal animal)
    {
        return $"{animal.Name} ({animal.Species}), {animal.Age} років";
    }
}