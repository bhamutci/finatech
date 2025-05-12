namespace FinaTech.Application.Services.Account.Dto;

public record AccountDto(int Id, int BankId, int AddressId, string Name, string? AccountNumber, string Iban)
{ }
