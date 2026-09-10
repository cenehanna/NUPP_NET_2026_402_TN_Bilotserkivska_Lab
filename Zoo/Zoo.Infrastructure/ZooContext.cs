using Microsoft.EntityFrameworkCore;
using Zoo.Infrastructure.Models;

namespace Zoo.Infrastructure;

public class ZooContext : DbContext
{
    public ZooContext() { }

    public ZooContext(DbContextOptions<ZooContext> options) : base(options)
    {
    }

    public DbSet<AnimalModel> Animals { get; set; } = null!;
    public DbSet<MammalModel> Mammals { get; set; } = null!;
    public DbSet<BirdModel> Birds { get; set; } = null!;
    public DbSet<EnclosureModel> Enclosures { get; set; } = null!;
    public DbSet<AnimalDetailModel> AnimalDetails { get; set; } = null!;
    public DbSet<KeeperModel> Keepers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = "Host=localhost;Port=5432;Database=ZooDb;Username=postgres;Password=8gyrg8104ife";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPT Наслідування
        modelBuilder.Entity<AnimalModel>().ToTable("Animals");
        modelBuilder.Entity<MammalModel>().ToTable("Mammals");
        modelBuilder.Entity<BirdModel>().ToTable("Birds");

        // 1:N Enclosure -> Animals
        modelBuilder.Entity<EnclosureModel>()
            .HasMany(e => e.Animals)
            .WithOne(a => a.Enclosure)
            .HasForeignKey(a => a.EnclosureId)
            .OnDelete(DeleteBehavior.SetNull);

        // 1:1 Animal -> AnimalDetail
        modelBuilder.Entity<AnimalDetailModel>(entity =>
        {
            entity.ToTable("AnimalDetails");
            entity.HasKey(d => d.Id); // Використовуємо Id з IEntity

            // Ігноруємо Name для цієї таблиці, бо стовпчик відсутній в БД
            entity.Ignore(d => d.Name);
        });

        modelBuilder.Entity<AnimalModel>()
            .HasOne(a => a.Detail)
            .WithOne(d => d.Animal)
            .HasForeignKey<AnimalDetailModel>(d => d.Id); // Вказуємо d.Id як FK

        // N:N Animals <-> Keepers
        // N:N Animals <-> Keepers (Явно вказуємо назву таблиці AnimalKeeper)
        modelBuilder.Entity<AnimalModel>()
            .HasMany(a => a.Keepers)
            .WithMany(k => k.Animals)
            .UsingEntity(j => j.ToTable("AnimalKeeper"));
    }
}