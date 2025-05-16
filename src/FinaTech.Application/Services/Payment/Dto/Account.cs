namespace FinaTech.Application.Services.Payment.Dto;

public record Account(
    int Id,
    int AddressId,
    string Name,
    string Iban,
    string Bic,
    string? AccountNumber,
    Address? Address)
{ }
