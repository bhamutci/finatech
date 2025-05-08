using Microsoft.EntityFrameworkCore;

namespace FinaTech.EntityFramework.PostgresSqlServer;

public sealed class FinaTechPostgresSqlDbContext: FinaTechDbContextBase<FinaTechPostgresSqlDbContext>
{
    /// <summary>
    /// Represents the PostgreSQL-specific database context for FinaTech,
    /// inheriting functionality for handling payments, accounts, banks, and addresses
    /// from the base FinaTechDbContextBase class. This class is configured to utilize
    /// Npgsql-specific settings for legacy timestamp behavior and disabling infinity conversions in DateTime types.
    /// </summary>
    public FinaTechPostgresSqlDbContext(DbContextOptions<FinaTechPostgresSqlDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

}
