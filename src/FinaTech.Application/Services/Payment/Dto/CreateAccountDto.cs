namespace FinaTech.Application.Services.Payment.Dto;

public record CreateAccountDto(
    string Name,
    string Iban,
    string Bic,
    string AccountNumber,
    CreateAddressDto Address)
{
}
