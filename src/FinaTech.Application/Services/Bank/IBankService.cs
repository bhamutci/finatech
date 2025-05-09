namespace FinaTech.Application.Services.Bank;

using Dto;

/// <summary>
/// Defines the contract for bank-related operations within the application.
/// Provides methods to retrieve, create, and manage bank details.
/// </summary>
public interface IBankService
{
    /// <summary>
    /// Retrieves detailed information about a specific bank based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the bank to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a <see cref="BankDto"/> object with detailed information about the bank.
    /// </returns>
    Task<BankDto> GetBankAsync(int id);

    /// <summary>
    /// Retrieves a list of all available banks with their detailed information.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a read-only list of <see cref="BankDto"/> objects,
    /// each providing detailed information about a bank.
    /// </returns>
    Task<IReadOnlyList<BankDto>> GetBanksAsync();

    /// <summary>
    /// Creates a new bank record based on the provided bank details.
    /// </summary>
    /// <param name="bank">The <see cref="BankDto"/> containing the details of the bank to be created.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains the created <see cref="BankDto"/> with the details of the new bank.
    /// </returns>
    Task<BankDto> CreateBankAsync(BankDto bank);
}
