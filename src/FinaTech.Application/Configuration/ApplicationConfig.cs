namespace FinaTech.Application.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


public static class ApplicationConfig
{
    public static IServiceCollection AddApplicationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationOptions>(options=> configuration.GetSection(ApplicationOptions.Name).Bind(options));

        return services;
    }
}
