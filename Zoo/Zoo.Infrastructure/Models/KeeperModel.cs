using System;
using System.Collections.Generic;
using Zoo.Common.Interfaces;

namespace Zoo.Infrastructure.Models
{
    public class KeeperModel : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public List<AnimalModel> Animals { get; set; } = new();
    }
}
