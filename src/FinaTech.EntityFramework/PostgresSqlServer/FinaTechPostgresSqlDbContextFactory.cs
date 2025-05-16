using FinaTech.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using FinaTech.EntityFramework.PostgresSqlServer;

namespace FinaTech.EntityFramework;

/// <summary>
/// Factory class to create instances of <see cref="FinaTechPostgresSqlDbContext"/> at design time.
/// This class implements <see cref="IDesignTimeDbContextFactory{TContext}"/> to facilitate the
/// configuration and creation of a database context for migrations or tooling purposes, primarily
/// targeting a PostgreSQL database.
/// </summary>
/// <remarks>
/// The factory uses a configuration-based approach to set up the PostgreSQL connection string,
/// accessing it through the application's configuration file. The returned context is configured
/// with the appropriate <see cref="DbContextOptions{TContext}"/> for PostgreSQL access.
/// This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else
/// </remarks>
public sealed class FinaTechPostgresSqlDbContextFactory: IDesignTimeDbContextFactory<FinaTechPostgresSqlDbContext>
{
    public FinaTechPostgresSqlDbContext CreateDbContext(string[] args)
    {
        var configuration = ApplicationConfigurations.Get(Directory.GetCurrentDirectory());

        var builder = new DbContextOptionsBuilder<FinaTechPostgresSqlDbContext>();

        builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new FinaTechPostgresSqlDbContext(builder.Options);
    }
}
