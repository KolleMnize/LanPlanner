using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanPlanner.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LanPlanner.Domain.Users;
using LanPlanner.Infrastructure.Persistence.Users;

namespace LanPlanner.Infrastructure.Installer;

public static class DependencyInjection
{
    public static void RegisterInfrastructureServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LanPlannerDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
