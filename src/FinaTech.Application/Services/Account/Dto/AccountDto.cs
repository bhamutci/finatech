namespace FinaTech.Application.Services.Account.Dto;

public record AccountDto(
    int Id,
    int AddressId,
    string Name,
    string AccountNumber,
    string Iban,
    string BIC,
    AddressDto? Address)
{ }
