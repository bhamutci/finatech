namespace FinaTech.Application.Services.Account.Dto;

public record CreateAccountDto(
    int Id,
    int BankId,
    int AddressId,
    string Name,
    string? AccountNumber,
    string Iban,
    AddressDto? Address)
{
}
