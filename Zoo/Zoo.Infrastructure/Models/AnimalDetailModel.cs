using System;
using Zoo.Common.Interfaces;

namespace Zoo.Infrastructure.Models
{
    public class AnimalDetailModel : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? MedicalNotes { get; set; }

        public AnimalModel? Animal { get; set; }
    }
}