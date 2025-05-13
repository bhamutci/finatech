namespace FinaTech.Application.Services.Bank;

using FinaTech.Application.Services.Dto;
using Dto;

/// <summary>
/// Provides methods to manage and retrieve bank information.
/// </summary>
public interface IBankService
{
    /// <summary>
    /// Retrieves detailed information about a specific bank based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the bank to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a <see cref="BankDto"/> with the detailed information of the requested bank.
    /// </returns>
    Task<BankDto> GetBankAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of banks based on the specified filter criteria.
    /// </summary>
    /// <param name="bankFilter">The filter criteria used for querying the banks.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a <see cref="PagedResultDto{BankDto}"/> with the paginated banks that meet the filter criteria.
    /// </returns>
    Task<PagedResultDto<BankDto>> GetBanksAsync(BankFilter bankFilter, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new bank with the provided details.
    /// </summary>
    /// <param name="bank">The data transfer object containing the details of the bank to create.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a <see cref="BankDto"/> representing the details of the newly created bank.
    /// </returns>
    Task<BankDto> CreateBankAsync(BankDto bank, CancellationToken cancellationToken);
}
