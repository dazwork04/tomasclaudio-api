using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Entities.Sbo;

namespace SAPB1SLayerWebAPI.Context
{
    public class SboDbContext: DbContext
    {
        public SboDbContext(DbContextOptions<SboDbContext> options) : base(options)
        {

        }
        public DbSet<SRGC> SRGC { get; set; }
    }
}
