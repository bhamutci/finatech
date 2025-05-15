namespace FinaTech.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostgresSqlServer;

/// <summary>
/// Provides an extension method for configuring and adding Entity Framework support
/// to the application's dependency injection container.
/// </summary>
/// <remarks>
/// This module integrates with Microsoft.EntityFrameworkCore and utilizes PostgreSQL
/// as the underlying database. It sets up the DbContext with a connection string
/// retrieved from IConfiguration and includes additional properties for context-specific behavior.
/// </remarks>
public static class EntityFrameworkModule
{
    /// <summary>
    /// Configures and adds Entity Framework Core support to the application's dependency injection container
    /// using PostgreSQL as the database provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration, providing access to the connection string.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> instance to allow for method chaining.
    /// </returns>
    public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FinaTechPostgresSqlDbContext>(builder =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseNpgsql(connectionString);
        });
    }
}
