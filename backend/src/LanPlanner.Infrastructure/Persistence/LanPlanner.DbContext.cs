namespace LanPlanner.Infrastructure.Persistence;

using LanPlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
public class LanPlannerDbContext : DbContext
{
    internal DbSet<User> Users => Set<User>();

    public LanPlannerDbContext(DbContextOptions<LanPlannerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LanPlannerDbContext).Assembly);
    }

}
