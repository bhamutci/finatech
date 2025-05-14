namespace FinaTech.Application.Services;

using Microsoft.Extensions.Logging;
using AutoMapper;
using EntityFramework.PostgresSqlServer;

public abstract class BaseApplicationService
{
    #region Fields

    /// <summary>
    /// Represents the database context used for interacting with the application's database.
    /// Provides access to database entities such as Payments.
    /// </summary>
    protected readonly FinaTechPostgresSqlDbContext dbContext;

    /// <summary>
    /// Provides object-object mapping functionality within the payment service,
    /// allowing transformations between domain models and DTOs.
    /// </summary>
    protected readonly IMapper mapper;

    /// <summary>
    /// Provides logging functionality for the application service,
    /// enabling the capture of detailed diagnostic and operational information.
    /// </summary>
    protected readonly ILogger<BaseApplicationService> logger;

    #endregion

    #region Constructors

    /// <summary>
    /// Serves as a base class for application services within the FinaTech system.
    /// This class provides shared functionality and dependencies required for services
    /// interacting with the database and performing object mapping.
    /// </summary>
    protected BaseApplicationService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper,
        ILogger<BaseApplicationService> logger)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.logger = logger;
    }

    #endregion
}
