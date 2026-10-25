using Microsoft.EntityFrameworkCore;
using JourneyViewer.Models;

namespace JourneyViewer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flow> Flows { get; set; }
        public DbSet<EDSCrmMapping> EDSCrmMappings { get; set; }
    }
}