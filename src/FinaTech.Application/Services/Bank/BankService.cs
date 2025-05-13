namespace FinaTech.Application.Services.Bank;

using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

using FinaTech.Application.Services.Dto;
using EntityFramework.PostgresSqlServer;
using Dto;
using Exceptions;

/// <summary>
/// Represents operations for managing and retrieving bank-related information within the application.
/// </summary>
public class BankService : BaseApplicationService, IBankService
{

    #region Constructors

    /// <summary>
    /// Provides functionalities for managing banks, including retrieving, creating,
    /// and handling related operations within the banking domain.
    /// </summary>
    public BankService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper, Logger<BankService> logger) : base(
        dbContext, mapper, logger)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asynchronously retrieves information about a specific bank by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the bank to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="BankDto"/>
    /// representing the bank information, mapped from the database entity.
    /// </returns>
    /// <exception cref="PaymentException">
    /// Thrown if a database error occurs or an unexpected error takes place while retrieving the bank information.
    /// </exception>
    public async Task<BankDto> GetBankAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve bank with ID: {BankId}", id);
        try
        {
            var bankEntity = await dbContext.Banks.FindAsync([id], cancellationToken);

            if (bankEntity == null)
            {
                logger.LogWarning("Bank with ID: {BankId} not found.", id);
            }

            return mapper.Map<BankDto>(bankEntity);

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetBankAsync for ID {BankId} was cancelled.", id);
            throw; // Re-throw the cancellation exception
        }
        catch (DbException dbEx) // Catch specific database exceptions
        {
            logger.LogError(dbEx, "Database error while retrieving bank with ID: {BankId}", id); // Log database error
            throw new PaymentException($"Could not retrieve bank with ID {id} due to a database error.",
                dbEx); // Throw custom exception
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving bank with ID: {BankId}", id);
            throw new PaymentException($"An unexpected error occurred while retrieving bank with ID {id}.", ex);
        }
    }

    /// <summary>
    /// Asynchronously retrieves a collection of banks from the database
    /// and returns them as a read-only list of bank data transfer objects (DTOs).
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a read-only list of <see cref="BankDto"/> objects representing the banks.</returns>
    public async Task<PagedResultDto<BankDto>> GetBanksAsync(BankFilter bankFilter, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve banks with filter: Keywords='{Keywords}', Skip={Skip}, Take={Take}",
            bankFilter?.Keywords, bankFilter?.SkipCount, bankFilter?.MaxResultCount);

        try
        {
            IQueryable<Core.Bank> bankQuery = GetBanksQuery(bankFilter, cancellationToken);

            int totalCount = await bankQuery.CountAsync(cancellationToken);

            List<Core.Bank> banks = await bankQuery.Skip(bankFilter.SkipCount)
                .Take(bankFilter.MaxResultCount)
                .ToListAsync(cancellationToken);

            IReadOnlyList<BankDto> bankDtos =
                mapper.Map<IReadOnlyList<Core.Bank>, IReadOnlyList<BankDto>>(banks);

            var pagedResultDto = new PagedResultDto<BankDto>
            {
                Items = bankDtos,
                TotalCount = totalCount
            };

            logger.LogInformation("Retrieved {BankCount} banks out of {TotalCount} total banks.",
                bankDtos.Count, totalCount);

            return pagedResultDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetBanksAsync was cancelled with filter: {Filter}", bankFilter);
            throw;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while retrieving banks with filter: {Filter}", bankFilter);
            throw new BankException("Could not retrieve banks due to a database error.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving banks with filter: {Filter}", bankFilter);
            throw new BankException("An unexpected error occurred while retrieving banks.", ex);
        }
    }


    /// <summary>
    /// Creates a new bank and returns the created bank details.
    /// </summary>
    /// <param name="bank">The <see cref="BankDto"/> object representing the bank to be created.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="BankDto"/> containing details of the bank that was created.</returns>
    /// <exception cref="BankException">Thrown when an unexpected error occurs during bank creation.</exception>
    /// <exception cref="ArgumentException">Thrown when bank input is invalid.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    public async Task<BankDto> CreateBankAsync(BankDto bank, CancellationToken cancellationToken)
    {
        try
        {
            await ValidateBankAsync(bank, cancellationToken);

            var bankDto = await SaveBankAsync(bank, cancellationToken);

            logger.LogInformation("Bank created successfully with ID: {BankID}", bankDto.Id);

            return bankDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreateBankAsync for Name {Name} was cancelled.", bank?.Name);
            throw;
        }
        catch (ArgumentException argEx)
        {
            logger.LogWarning(argEx, "Bank creation failed due to invalid input: {Message}", argEx.Message);
            throw;
        }
        catch (BankException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while creating a bank. Name: {Name}", bank?.Name);
            throw new BankException("An unexpected error occurred while creating the bank.", ex);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Saves a bank entity to the database by mapping the provided BankDto to a Core.Bank entity,
    /// handling persistence, and returning a PaymentDto representation of the saved entity.
    /// </summary>
    /// <param name="bank">The BankDto object containing the bank details to be saved.</param>
    /// <param name="cancellationToken">A CancellationToken to observe for task cancellation requests.</param>
    /// <returns>A PaymentDto representation of the saved bank entity.</returns>
    /// <exception cref="BankAlreadyExistsException">Thrown when a bank with the same unique constraints already exists.</exception>
    /// <exception cref="BankException">Thrown when an error occurs while saving the bank due to database or unexpected issues.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    private async Task<BankDto> SaveBankAsync(BankDto bank, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mapping BankDto to Bank entity for saving.");
        try
        {
            Core.Bank bankEntity = mapper.Map<BankDto, Core.Bank>(bank);

            await dbContext.Banks.AddAsync(bankEntity, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<Core.Bank, BankDto>(bankEntity);

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("SaveBankAsync for Name {Name} was cancelled.", bank?.Name);
            throw;
        }
        catch (DbUpdateException dbUpdateEx)
        {
            logger.LogError(dbUpdateEx, "Database update error while saving bank with Name: {Name}", bank?.Name);

            if (dbUpdateEx.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new BankAlreadyExistsException("A bank with this name exists.", dbUpdateEx);
            }

            throw new BankException($"Could not save bank with Name {bank?.Name} due to a database update error.",
                dbUpdateEx);
        }
        catch (DbException dbEx) // Catch other general database errors (connection, etc.)
        {
            logger.LogError(dbEx, "A general database error occurred while saving bank with Name: {Name}", bank?.Name);
            throw new BankException($"A database error occurred while saving bank with Name {bank?.Name}.", dbEx);
        }
        catch (Exception ex) // Catch any other unexpected errors during save
        {
            logger.LogError(ex,
                "An unexpected error occurred while saving bank with Name: {Name}",
                bank?.Name);
            throw new BankException(
                $"An unexpected error occurred while saving bank with Name {bank?.Name}.",
                ex);
        }
    }

    /// <summary>
    /// Constructs a query to retrieve a collection of banks from the database based on
    /// the specified filter criteria, allowing further customization or execution.
    /// </summary>
    /// <param name="bankFilter">The filter containing criteria to apply when querying banks.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the filtered banks query.</returns>
    private IQueryable<Core.Bank> GetBanksQuery(BankFilter bankFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogDebug("Building bank query with filter: {Filter}", bankFilter);

        var banks = dbContext.Banks
            .Include(p => p.Accounts)
            .AsQueryable();

        if (bankFilter != null)
        {
            if (bankFilter.SkipCount < 0)
            {
                throw new ArgumentException("Skip count cannot be less than zero.", nameof(bankFilter.SkipCount));
            }

            if (!string.IsNullOrEmpty(bankFilter.Keywords))
            {
                logger.LogDebug("Applying keyword filter: {Keywords}", bankFilter.Keywords);
                banks = banks.Where(p => p.Name != null && p.Name.Contains(bankFilter.Keywords) ||
                                         p.BIC != null && p.BIC.Contains(bankFilter.Keywords));
            }
        }

        return banks;
    }

    /// <summary>
    /// Validates the provided bank details, ensuring all required fields are specified and valid.
    /// </summary>
    /// <param name="bank">The bank object containing the details to be validated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bank object or any required property is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the bank ID is incorrectly specified.</exception>
    private Task ValidateBankAsync(BankDto bank, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation("Starting validation for payment: {Name}", bank?.Name);

        if (bank == null)
        {
            logger.LogWarning("Validation failed: Bank details cannot be null.");
            throw new ArgumentNullException(nameof(bank), "Bank details cannot be null.");
        }

        if (string.IsNullOrEmpty(bank.Name))
        {
            logger.LogWarning("Validation failed: Bank name cannot be null or empty.");
            throw new ArgumentNullException(nameof(bank.Name), "Bank name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(bank.BIC))
        {
            logger.LogWarning("Validation failed: Bank BIC cannot be null or empty.");
            throw new ArgumentNullException(nameof(bank.BIC), "Bank BIC cannot be null or empty.");
        }

        logger.LogDebug("Bank validation completed successfully.");
        return Task.CompletedTask;
    }

    #endregion
}
