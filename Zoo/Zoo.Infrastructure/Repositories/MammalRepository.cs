using Microsoft.EntityFrameworkCore;
using Zoo.Infrastructure.Models;

namespace Zoo.Infrastructure.Repositories;

public class MammalRepository : GenericRepository<MammalModel>
{
    public MammalRepository(ZooContext context) : base(context) { }

    public override async Task<IEnumerable<MammalModel>> GetAllAsync()
    {
        return await _context.Mammals
            .Include(m => m.Keepers)
            .Include(m => m.Detail)
            .Include(m => m.Enclosure)
            .ToListAsync();
    }
}