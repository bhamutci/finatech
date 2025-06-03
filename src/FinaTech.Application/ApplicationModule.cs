namespace FinaTech.Application;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Mapper;
using Configuration;
using EntityFramework;
using Services.Payment;
using Services.Payment.Dto;
using Services.Payment.Dto.Validator;
using FluentValidation;

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
    public static IServiceCollection AddPaymentApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationConfig(configuration)
            .AddEntityFramework(configuration)
            .AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly)
            .AddScoped<IValidator<CreateAccount>, CreateAccountDtoValidator>()
            .AddScoped<IValidator<CreatePayment>, CreatePaymentDtoValidator>()
            .AddScoped<IValidator<CreateAddress>, AddressDtoValidator>()
            .AddScoped<IValidator<Money>, MoneyDtoValidator>()
            .AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
