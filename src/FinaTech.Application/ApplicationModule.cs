namespace FinaTech.Application;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Mapper;
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
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFramework(configuration);
        services.AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly);

        services.AddScoped<IValidator<CreateAccountDto>, CreateAccountDtoValidator>();
        services.AddScoped<IValidator<CreatePaymentDto>, CreatePaymentDtoValidator>();
        services.AddScoped<IValidator<CreateAddressDto>, AddressDtoValidator>();
        services.AddScoped<IValidator<MoneyDto>, MoneyDtoValidator>();
        services.AddScoped<IPaymentService, PaymentService>();
    }
}
