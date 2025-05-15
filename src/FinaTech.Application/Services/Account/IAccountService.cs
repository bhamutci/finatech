namespace FinaTech.Application.Services.Account;

using Dto;
using FinaTech.Application.Services.Dto;

public interface IAccountService
{
    /// <summary>
    /// Retrieves the details of a specific account by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the account to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains an <see cref="AccountDto"/> representing the account details.
    /// </returns>
    Task<AccountDto> GetAccountAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of accounts based on the specified filter criteria.
    /// </summary>
    /// <param name="accountFilter">The filter criteria to apply when retrieving the accounts.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a <see cref="PagedResultDto{AccountDto}"/> with the list of accounts and total count.
    /// </returns>
    Task<PagedResultDto<AccountDto>> GetAccountsAsync(AccountFilter? accountFilter, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new account with the provided details.
    /// </summary>
    /// <param name="account">The account details to create, represented as an <see cref="CreateAccountDto"/>.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains the created <see cref="CreateAccountDto"/> with the account details.
    /// </returns>
    Task<AccountDto> CreateAccountAsync(CreateAccountDto account, CancellationToken cancellationToken);
}
