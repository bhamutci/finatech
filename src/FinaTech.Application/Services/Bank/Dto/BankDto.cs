namespace FinaTech.Application.Services.Bank.Dto;

using FinaTech.Application.Services.Account.Dto;


/// <summary>
/// Represents the data transfer object for a bank, containing essential bank information.
/// </summary>
public record BankDto(int Id,
    string Name,
    string BIC,
    ICollection<AccountDto> Accounts)
{ }
