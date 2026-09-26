using System;
using System.Collections.Generic;
using Zoo.Common;

namespace Zoo.Infrastructure.Models
{
    public class AnimalModel : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public int Age { get; set; }

        public Guid? EnclosureId { get; set; }
        public EnclosureModel? Enclosure { get; set; }

        public AnimalDetailModel? Detail { get; set; }

        public List<KeeperModel> Keepers { get; set; } = new();
    }
}
