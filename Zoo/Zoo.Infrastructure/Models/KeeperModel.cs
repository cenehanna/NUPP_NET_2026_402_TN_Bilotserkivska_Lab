using System;
using System.Collections.Generic;

namespace Zoo.Infrastructure.Models
{
    public class KeeperModel : Zoo.Common.IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public List<AnimalModel> Animals { get; set; } = new();
    }
}
