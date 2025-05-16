namespace FinaTech.Application.Services.Payment.Dto;

public record CreateAccount(
    string Name,
    string Iban,
    string Bic,
    string AccountNumber,
    CreateAddress Address)
{
}
