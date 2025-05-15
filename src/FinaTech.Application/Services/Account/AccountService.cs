namespace FinaTech.Application.Services.Account;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Dto;
using Exceptions;
using FinaTech.Application.Services.Dto;
using EntityFramework.PostgresSqlServer;

/// <summary>
/// Represents a service for managing accounts in the banking domain.
/// Provides methods to retrieve, create, and manage account-related operations.
/// </summary>
public class AccountService : BaseApplicationService, IAccountService
{
    #region Constructors

    /// <summary>
    /// Provides functionalities for managing banks, including retrieving, creating,
    /// and handling related operations within the banking domain.
    /// </summary>
    public AccountService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper, ILogger<AccountService> logger) : base(
        dbContext, mapper, logger)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asynchronously retrieves account details based on the specified account ID.
    /// </summary>
    /// <param name="id">The unique identifier of the account to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the details of the account as an <see cref="AccountDto"/> object.</returns>
    /// <exception cref="AccountException">
    /// Thrown if there is an error retrieving the account, such as a database issue or unexpected exception.
    /// </exception>
    public async Task<AccountDto> GetAccountAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve account with ID: {AccountId}", id);
        try
        {
            var accountEntity = await dbContext.Accounts.FindAsync([id], cancellationToken);

            if (accountEntity == null)
            {
                logger.LogWarning("Account with ID: {AccountId} not found.", id);
            }

            return mapper.Map<AccountDto>(accountEntity);

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetAccountAsync for ID {AccountId} was cancelled.", id);
            throw; // Re-throw the cancellation exception
        }
        catch (DbException dbEx) // Catch specific database exceptions
        {
            logger.LogError(dbEx, "Database error while retrieving account with ID: {AccountID}", id); // Log database error
            throw new AccountException($"Could not retrieve account with ID {id} due to a database error.",
                dbEx); // Throw a custom exception
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving account with ID: {AccountID}", id);
            throw new AccountException($"An unexpected error occurred while retrieving account with ID {id}.", ex);
        }
    }

    /// <summary>
    /// Retrieves a paginated list of accounts based on the provided filter criteria.
    /// </summary>
    /// <param name="accountFilter">The filter criteria used to refine the account list, including pagination and search keywords.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A paginated result containing a list of accounts and additional metadata.</returns>
    /// <exception cref="AccountException">Thrown when errors occur specific to account retrieval.</exception>
    public async Task<PagedResultDto<AccountDto>> GetAccountsAsync(AccountFilter? accountFilter,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Attempting to retrieve banks with filter: Keywords='{Keywords}', Skip={Skip}, Take={Take}",
            accountFilter?.Keywords, accountFilter?.SkipCount, accountFilter?.MaxResultCount);

        try
        {
            ArgumentNullException.ThrowIfNull(accountFilter);

            IQueryable<Core.Account> accountQuery = GetAccountQuery(accountFilter, cancellationToken);

            List<Core.Account> accounts = await accountQuery
                .Skip(accountFilter.SkipCount)
                .Take(accountFilter.MaxResultCount)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var totalCount = await accountQuery.CountAsync(cancellationToken);

            var accountDtos =
                mapper.Map<List<Core.Account>, IReadOnlyList<AccountDto>>(accounts);

            var pagedResultDto = new PagedResultDto<AccountDto>
            {
                Items = accountDtos,
                TotalCount = totalCount
            };

            logger.LogInformation("Retrieved {AccountCount} accounts out of {TotalCount} total accounts.",
                accountDtos?.Count, totalCount);

            return pagedResultDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetAccountAsync was cancelled with filter: {Filter}", accountFilter);
            throw;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while retrieving accounts with filter: {Filter}", accountFilter);
            throw new AccountException("Could not retrieve accounts due to a database error.", dbEx);
        }
        catch (ArgumentNullException argEx)
        {
            logger.LogError(argEx, "Invalid filter provided: {Filter}", accountFilter);
            throw new AccountException("Invalid filter provided.", argEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving accounts with filter: {Filter}", accountFilter);
            throw new AccountException("An unexpected error occurred while retrieving accounts.", ex);
        }
    }

    /// <summary>
    /// Asynchronously creates a new account based on the provided account information.
    /// </summary>
    /// <param name="account">An instance of <see cref="CreateAccountDto"/> containing the details of the account to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An instance of <see cref="AccountDto"/> representing the newly created account.</returns>
    /// <exception cref="AccountAlreadyExistsException">Thrown when an account with the same details already exists.</exception>
    /// <exception cref="AccountException">Thrown when an error occurs during account creation.</exception>
    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto? account, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mapping AccountDto to Account entity for saving.");
        try
        {
            return await SaveAccountAsync(account, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreateAccountAsync for Name {Name} was cancelled.", account?.Name);
            throw;
        }
        catch (DbUpdateException dbUpdateEx)
        {
            logger.LogError(dbUpdateEx, "Database update error while saving account with Name: {Name}", account?.Name);

            if (dbUpdateEx.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new AccountAlreadyExistsException("An Account with this name exists.", dbUpdateEx);
            }

            throw new AccountException($"Could not save account with Name {account?.Name} due to a database update error.",
                dbUpdateEx);
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "A general database error occurred while saving account with Name: {Name}", account?.Name);
            throw new AccountException($"A database error occurred while saving account with Name {account?.Name}.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while saving account with Name: {Name}",
                account?.Name);
            throw new AccountException(
                $"An unexpected error occurred while saving account with Name {account?.Name}.",
                ex);
        }
    }

    #endregion

    #region Private Methods

    private async Task<AccountDto> SaveAccountAsync(CreateAccountDto? account, CancellationToken cancellationToken)
    {
        if (account == null)
        {
            logger.LogWarning("Validation failed: Account cannot be null.");
            throw new ArgumentNullException(nameof(account), "Account cannot be null.");
        }

        ValidateBank(account, cancellationToken);

        logger.LogDebug("Mapping AccountDto to Account entity for saving.");

        Core.Account accountEntity = mapper.Map<CreateAccountDto, Core.Account>(account);

        await dbContext.Accounts.AddAsync(accountEntity, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Accounts.Entry(accountEntity).ReloadAsync(cancellationToken);

        logger.LogDebug("Account created completed successfully.");

        return mapper.Map<Core.Account, AccountDto>(accountEntity);
    }

    /// <summary>
    /// Builds and retrieves a queryable collection of accounts based on the provided account filter
    /// while supporting cancellation through a token. Applies filters for account search,
    /// including skip count and keyword criteria.
    /// </summary>
    /// <param name="accountFilter">The filter criteria for querying accounts.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An IQueryable sequence of accounts based on the applied filter.</returns>
    /// <exception cref="ArgumentException">Thrown when invalid parameters are provided,
    /// such as a negative skip count.</exception>
    private IQueryable<Core.Account> GetAccountQuery(AccountFilter? accountFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogDebug("Building account query with filter: {Filter}", accountFilter);

        var accounts = dbContext.Accounts
            .Include(p=>p.Address)
            .AsQueryable();

        if (accountFilter == null) return accounts;

        if (accountFilter.SkipCount < 0)
        {
            throw new ArgumentException("Skip count cannot be less than zero.", nameof(accountFilter.SkipCount));
        }

        if (string.IsNullOrEmpty(accountFilter.Keywords)) return accounts;

        logger.LogDebug("Applying keyword filter: {Keywords}", accountFilter.Keywords);

        accounts = accounts.Where(p => p.Address != null && (p.Name.Contains(accountFilter.Keywords) ||
                                                             p.Iban.Contains(accountFilter.Keywords) ||
                                                             p.Address.AddressLine1.Contains(accountFilter.Keywords) ||
                                                             p.Address.AddressLine2 != null && p.Address.AddressLine2.Contains(accountFilter.Keywords)||
                                                             p.Address.AddressLine3 != null && p.Address.AddressLine3.Contains(accountFilter.Keywords) ||
                                                             p.Address.City != null && p.Address.City.Contains(accountFilter.Keywords) ||
                                                             p.Address.PostCode != null && p.Address.PostCode.Contains(accountFilter.Keywords) ||
                                                             p.Address.CountryCode.Contains(accountFilter.Keywords) ||
                                                             p.AccountNumber != null && p.AccountNumber.Contains(accountFilter.Keywords)));

        return accounts;
    }

    /// <summary>
    /// Validates the provided account details to ensure that required fields are not null or empty.
    /// Throws exceptions if validation fails.
    /// </summary>
    /// <param name="account">The account information to validate, including its name, IBAN, and address.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when required fields (e.g., Name, IBAN, or Address) are null or empty.</exception>
    private void ValidateBank(CreateAccountDto? account, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation("Starting validation for account: {Name}", account?.Name);

        if (account == null)
        {
            logger.LogWarning("Validation failed: Account cannot be null.");
            throw new ArgumentNullException(nameof(account), "Account cannot be null.");
        }

        if (string.IsNullOrEmpty(account.Name))
        {
            logger.LogWarning("Validation failed: Account name cannot be null or empty.");
            throw new ArgumentNullException(nameof(account.Name), "Account name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(account.Iban))
        {
            logger.LogWarning("Validation failed: Account IBAN cannot be null or empty.");
            throw new ArgumentNullException(nameof(account.Iban), "Account IBAN cannot be null or empty.");
        }

        if (account.Address == null)
        {
            logger.LogWarning("Validation failed: Account Address cannot be null or empty.");
            throw new ArgumentNullException(nameof(account.Address), "Account Address cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(account.Address.CountryCode))
        {
            logger.LogWarning("Validation failed: Account Address Country cannot be null or empty.");
            throw new ArgumentNullException(nameof(account.Address.CountryCode), "Account Address Country cannot be null or empty.");
        }

        logger.LogDebug("Account  validation completed successfully.");
    }

    #endregion
}
