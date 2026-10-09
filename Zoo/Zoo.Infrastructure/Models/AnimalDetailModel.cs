using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zoo.Common.Interfaces;

namespace Zoo.Infrastructure.Models
{
    public class AnimalDetailModel : IEntity
    {
        [Key]
        [ForeignKey("Animal")]
        [Column("AnimalId")]
        public Guid Id { get; set; }

        public string? MedicalNotes { get; set; }

        public string? Name { get; set; }

        public AnimalModel? Animal { get; set; }
    }
}
