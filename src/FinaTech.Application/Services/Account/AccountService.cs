namespace FinaTech.Application.Services.Account;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Dto;
using Exceptions;
using FinaTech.Application.Services.Dto;
using EntityFramework.PostgresSqlServer;


public class AccountService: BaseApplicationService, IAccountService
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
            var AccountEntity = await dbContext.Accounts.FindAsync([id], cancellationToken);

            if (AccountEntity == null)
            {
                logger.LogWarning("Account with ID: {AccountId} not found.", id);
            }

            return mapper.Map<AccountDto>(AccountEntity);

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
                dbEx); // Throw custom exception
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving account with ID: {AccountID}", id);
            throw new AccountException($"An unexpected error occurred while retrieving account with ID {id}.", ex);
        }
    }

    /// <summary>
    /// Asynchronously retrieves a paginated list of accounts based on the provided filter criteria.
    /// </summary>
    /// <param name="accountFilter">The filter criteria to apply while retrieving accounts, including keywords and pagination options.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PagedResultDto{AccountDto}"/> containing a list of accounts and the total count of matching accounts.</returns>
    /// <exception cref="AccountException">Thrown when an error occurs while retrieving accounts, such as database errors or unexpected issues.</exception>
    public async Task<PagedResultDto<AccountDto>> GetAccountsAsync(AccountFilter accountFilter,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Attempting to retrieve banks with filter: Keywords='{Keywords}', Skip={Skip}, Take={Take}",
            accountFilter?.Keywords, accountFilter?.SkipCount, accountFilter?.MaxResultCount);

        try
        {
            IQueryable<Core.Account> accountQuery = GetAccountQuery(accountFilter, cancellationToken);

            List<Core.Account> accounts = await accountQuery
                .Skip(accountFilter.SkipCount)
                .Take(accountFilter.MaxResultCount)
                .ToListAsync(cancellationToken);

            int totalCount = await accountQuery.CountAsync(cancellationToken);


            IReadOnlyList<AccountDto> AccountDtos =
                mapper.Map<List<Core.Account>, IReadOnlyList<AccountDto>>(accounts);

            var pagedResultDto = new PagedResultDto<AccountDto>
            {
                Items = AccountDtos,
                TotalCount = totalCount
            };

            logger.LogInformation("Retrieved {AccountCount} accounts out of {TotalCount} total accounts.",
                AccountDtos?.Count, totalCount);

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
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving accounts with filter: {Filter}", accountFilter);
            throw new AccountException("An unexpected error occurred while retrieving accounts.", ex);
        }
    }


    /// <summary>
    /// Creates a new account based on the provided account data and returns the created account details.
    /// </summary>
    /// <param name="account">The account data to be created, encapsulated in an <see cref="AccountDto"/> object.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete, enabling request cancellation.</param>
    /// <returns>The details of the newly created account as an <see cref="AccountDto"/>.</returns>
    /// <exception cref="BankException">Thrown when an unexpected error occurs during account creation.</exception>
    /// <exception cref="AccountException">Thrown when a domain-specific account error is encountered.</exception>
    /// <exception cref="ArgumentException">Thrown when invalid arguments are passed for account creation.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled through the provided cancellation token.</exception>
    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto account, CancellationToken cancellationToken)
    {
        try
        {
            ValidateBankAsync(account, cancellationToken);

            var accountDto = await SaveAccountAsync(account, cancellationToken);

            logger.LogInformation("Account created successfully with ID: {AccountID}", accountDto.Id);

            return accountDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreateAccountAsync for Name {Name} was cancelled.", account?.Name);
            throw;
        }
        catch (ArgumentException argEx)
        {
            logger.LogWarning(argEx, "Account creation failed due to invalid input: {Message}", argEx.Message);
            throw;
        }
        catch (AccountException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while creating an account. Name: {Name}", account?.Name);
            throw new AccountException("An unexpected error occurred while creating the account.", ex);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Asynchronously saves an account to the database, including mapping, validation,
    /// and error handling during the persistence process.
    /// </summary>
    /// <param name="account">The account information to be saved, encapsulated in an <see cref="AccountDto"/> object.</param>
    /// <param name="cancellationToken">The token used to propagate notification that the operation should be canceled.</param>
    /// <returns>An <see cref="CreateAccountDto"/> representing the saved account with updated data from the database.</returns>
    /// <exception cref="BankAlreadyExistsException">Thrown when the account conflicts with an existing bank entity.</exception>
    /// <exception cref="AccountException">Thrown when any unforeseen issue prevents the account from being saved successfully.</exception>
    private async Task<AccountDto> SaveAccountAsync(CreateAccountDto account, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mapping AccountDto to Account entity for saving.");
        try
        {
            Core.Account accountEntity = mapper.Map<CreateAccountDto, Core.Account>(account);

            await dbContext.Accounts.AddAsync(accountEntity, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await dbContext.Accounts.Entry(accountEntity).ReloadAsync(cancellationToken);

            return mapper.Map<Core.Account, AccountDto>(accountEntity);

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("SaveAccountAsync for Name {Name} was cancelled.", account?.Name);
            throw;
        }
        catch (DbUpdateException dbUpdateEx)
        {
            logger.LogError(dbUpdateEx, "Database update error while saving account with Name: {Name}", account?.Name);

            if (dbUpdateEx.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new BankAlreadyExistsException("An Account with this name exists.", dbUpdateEx);
            }

            throw new AccountException($"Could not save account with Name {account?.Name} due to a database update error.",
                dbUpdateEx);
        }
        catch (DbException dbEx) // Catch other general database errors (connection, etc.)
        {
            logger.LogError(dbEx, "A general database error occurred while saving account with Name: {Name}", account?.Name);
            throw new AccountException($"A database error occurred while saving account with Name {account?.Name}.", dbEx);
        }
        catch (Exception ex) // Catch any other unexpected errors during save
        {
            logger.LogError(ex,
                "An unexpected error occurred while saving account with Name: {Name}",
                account?.Name);
            throw new AccountException(
                $"An unexpected error occurred while saving account with Name {account?.Name}.",
                ex);
        }
    }

    /// <summary>
    /// Builds and retrieves a queryable collection of accounts based on the provided account filter
    /// while supporting cancellation through a token. Applies filters for account search
    /// including skip count and keyword criteria.
    /// </summary>
    /// <param name="accountFilter">The filter criteria for querying accounts.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An IQueryable sequence of accounts based on the applied filter.</returns>
    /// <exception cref="ArgumentException">Thrown when invalid parameters are provided
    /// such as a negative skip count.</exception>
    private IQueryable<Core.Account> GetAccountQuery(AccountFilter accountFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogDebug("Building account query with filter: {Filter}", accountFilter);

        var accounts = dbContext.Accounts
            .Include(p => p.Bank)
            .Include(p=>p.Address)
            .AsQueryable();

        if (accountFilter != null)
        {
            if (accountFilter.SkipCount < 0)
            {
                throw new ArgumentException("Skip count cannot be less than zero.", nameof(accountFilter.SkipCount));
            }

            if (!string.IsNullOrEmpty(accountFilter.Keywords))
            {
                logger.LogDebug("Applying keyword filter: {Keywords}", accountFilter.Keywords);
                accounts = accounts.Where(p => p.Name != null && p.Name.Contains(accountFilter.Keywords) ||
                                               p.Iban != null && p.Iban.Contains(accountFilter.Keywords) ||
                                               p.Address.AddressLine1 != null && p.Address.AddressLine1.Contains(accountFilter.Keywords) ||
                                               p.Address.AddressLine2 != null && p.Address.AddressLine2.Contains(accountFilter.Keywords)||
                                               p.Address.AddressLine3 != null && p.Address.AddressLine3.Contains(accountFilter.Keywords) ||
                                               p.Address.City != null && p.Address.City.Contains(accountFilter.Keywords) ||
                                               p.Address.PostCode != null && p.Address.PostCode.Contains(accountFilter.Keywords) ||
                                               p.Address.CountryCode != null && p.Address.CountryCode.Contains(accountFilter.Keywords) ||
                                               p.AccountNumber != null && p.AccountNumber.Contains(accountFilter.Keywords));
            }
        }

        return accounts;
    }

    /// <summary>
    /// Validates the provided bank details, ensuring all required fields are specified and valid.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="bank">The bank object containing the details to be validated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bank object or any required property is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the bank ID is incorrectly specified.</exception>
    private void ValidateBankAsync(CreateAccountDto account, CancellationToken cancellationToken)
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
