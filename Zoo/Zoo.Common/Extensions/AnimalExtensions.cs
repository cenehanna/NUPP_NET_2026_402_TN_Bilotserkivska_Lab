using Zoo.Common.Models;

namespace Zoo.Common.Extensions;

public static class AnimalExtensions
{
    // Метод розширення
    public static string GetShortInfo(this Animal animal)
    {
        return $"{animal.Name} ({animal.Species}), {animal.Age} років";
    }
}