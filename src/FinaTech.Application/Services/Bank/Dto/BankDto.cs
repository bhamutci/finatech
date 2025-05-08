using FinaTech.Core;

namespace FinaTech.Application.Services.Bank.Dto;

/// <summary>
/// Represents the data transfer object for a bank, containing essential bank information.
/// </summary>
public record BankDto(int Id,
    string Name,
    string BIC,
    ICollection<Account> Accounts)
{ }
