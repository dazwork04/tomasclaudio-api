using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Entities.Main;

namespace SAPB1SLayerWebAPI.Context
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> context) : base(context)
        {

        }
        public DbSet<OIBT> OIBT { get; set; }
        public DbSet<OWDD> OWDD { get; set; }
        public DbSet<JDT1> JDT1 { get; set; }
        public DbSet<OACT> OACT { get; set; }
        public DbSet<OPDF> OPDF { get; set; }
        public DbSet<ORCT> ORCT { get; set; }
        public DbSet<OVPM> OVPM { get; set; }
        public DbSet<OSRI> OSRI { get; set; }
        public DbSet<OUSR> OUSR { get; set; }

    }
}
