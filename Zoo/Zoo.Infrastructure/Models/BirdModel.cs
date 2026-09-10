using System;
using Zoo.Common;

namespace Zoo.Infrastructure.Models
{
    public class BirdModel : AnimalModel
    {
        public double Wingspan { get; set; }
        public bool CanFly { get; set; }
        public string? FeatherColor { get; set; }

        public static BirdModel CreateNew()
        {
            var rnd = Random.Shared;
            var age = rnd.Next(1, 15);
            var wingspan = Math.Round(rnd.NextDouble() * 2.5 + 0.2, 2);
            var names = new[] { "Кеша", "Орлик", "Чижик", "Сова", "Папуга" };
            var species = new[] { "Папуга", "Орел", "Сова", "Голуб", "Сокіл" };

            return new BirdModel
            {
                Id = Guid.NewGuid(),
                Name = names[rnd.Next(names.Length)],
                Species = species[rnd.Next(species.Length)],
                Age = age,
                Wingspan = wingspan,
                CanFly = true,
                FeatherColor = "Різнокольоровий"
            };
        }
    }
}
