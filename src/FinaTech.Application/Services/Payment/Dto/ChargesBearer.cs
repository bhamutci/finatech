namespace FinaTech.Application.Services.Payment.Dto;

/// <summary>
/// Represents the party responsible for bearing the charges associated with a financial transaction.
/// </summary>
public enum ChargesBearer
{
    Originator,
    Beneficiary,
    Shared
}
