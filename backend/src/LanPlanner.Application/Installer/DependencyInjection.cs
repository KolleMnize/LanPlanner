
using LanPlanner.Application.Features.Users.GetCurrentUser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LanPlanner.Application.Installer;

public class DependencyInjection
{
    public static void RegisterApplicationServices(IServiceCollection services, IHostEnvironment environment)
    {
        services.AddScoped<GetCurrentUserHandler>();
    }
}
