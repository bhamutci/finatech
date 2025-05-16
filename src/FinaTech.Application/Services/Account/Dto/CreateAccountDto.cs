namespace FinaTech.Application.Services.Account.Dto;

public record CreateAccountDto(
    string Name,
    string Iban,
    string Bic,
    string AccountNumber,
    AddressDto? Address)
{
}
