namespace FinaTech.Application.Services.Account.Dto;

public record CreateAccountDto(
    int Id,
    int AddressId,
    string Name,
    string? AccountNumber,
    string Iban,
    string Bic,
    AddressDto? Address)
{
}
