using System;
using System.Collections.Generic;

namespace Zoo.Infrastructure.Models
{
    public class EnclosureModel : Zoo.Common.IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double Area { get; set; }
        public int Capacity { get; set; }

        public List<AnimalModel> Animals { get; set; } = new();

        public static EnclosureModel CreateNew()
        {
            var rnd = Random.Shared;
            var area = Math.Round(rnd.NextDouble() * 200 + 20, 1);
            var capacity = rnd.Next(1, 10);
            var names = new[] { "Вольєр A", "Вольєр B", "Вольєр C" };

            return new EnclosureModel
            {
                Id = Guid.NewGuid(),
                Name = names[rnd.Next(names.Length)],
                Area = area,
                Capacity = capacity
            };
        }
    }
}
