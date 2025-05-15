namespace FinaTech.Application.Services.Account.Dto;

public record AccountDto(
    int Id,
    int AddressId,
    string Name,
    string Iban,
    string Bic,
    string? AccountNumber,
    AddressDto? Address)
{ }
