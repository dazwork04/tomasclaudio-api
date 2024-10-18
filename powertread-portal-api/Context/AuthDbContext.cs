using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Entities.Auth;

namespace SAPB1SLayerWebAPI.Context
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> context) : base(context)
        {

        }
        public DbSet<OUSR> OUSR { get; set; }
    }
}
