using System;
using Zoo.Common;

namespace Zoo.Infrastructure.Models
{
    public class MammalModel : AnimalModel
    {
        public string? FurColor { get; set; }
        public string? Diet { get; set; }
        public double Weight { get; set; }

        public static MammalModel CreateNew()
        {
            var rnd = Random.Shared;
            var age = rnd.Next(1, 20);
            var weight = Math.Round(rnd.NextDouble() * 200 + 20, 1);
            var names = new[] { "Сімба", "Мурчик", "Ричі", "Лео", "Балу" };
            var species = new[] { "Лев", "Тигр", "Ведмідь", "Пантера" };

            return new MammalModel
            {
                Id = Guid.NewGuid(),
                Name = names[rnd.Next(names.Length)],
                Species = species[rnd.Next(species.Length)],
                Age = age,
                FurColor = "Різний",
                Diet = "М'ясоїдний",
                Weight = weight
            };
        }

        public override string ToString()
        {
            return $"[Ссавець] {Name} | Вид: {Species} | Вік: {Age} | Вага: {Weight} кг | Хутро: {FurColor}";
        }
    }
}