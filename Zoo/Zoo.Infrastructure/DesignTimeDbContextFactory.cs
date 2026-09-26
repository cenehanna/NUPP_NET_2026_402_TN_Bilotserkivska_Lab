using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zoo.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ZooContext>
{
    public ZooContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ZooContext>();
        var connectionString = "Host=localhost;Port=5432;Database=ZooDb;Username=postgres;Password=8gyrg8104ife";
        optionsBuilder.UseNpgsql(connectionString);
        return new ZooContext(optionsBuilder.Options);
    }
}
