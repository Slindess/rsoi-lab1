using Microsoft.Extensions.DependencyInjection;
using PersonApp.Application.Services;
using PersonApp.Core.Interfaces.Services;

namespace PersonApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPersonService, PersonService>();
        return services;
    }
}
