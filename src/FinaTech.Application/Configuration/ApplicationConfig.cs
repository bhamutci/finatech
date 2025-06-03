namespace FinaTech.Application.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


public static class ApplicationConfig
{
    public static IServiceCollection AddApplicationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationOptions>(options=> configuration.GetSection("Application").Bind(options));

        return services;
    }
}
