using FinaTech.Application.Mapper;

namespace FinaTech.Application;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService;
using EntityFramework;

/// <summary>
/// Provides extension methods for configuring application-specific services in the dependency injection container.
/// </summary>
public static class ApplicationModule
{
    /// <summary>
    /// Adds application-specific services to the dependency injection container.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="configuration">The configuration object used to configure services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFramework(configuration);
        services.AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly);
        services.AddScoped<PaymentStrategy, SEPAPayment>();
        services.AddScoped<PaymentStrategy, SWIFTPayment>();
        services.AddScoped<IPaymentService, PaymentService.PaymentService>();

        return services;
    }
}
