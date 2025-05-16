namespace FinaTech.Application.Services.Payment.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a payment.
/// </summary>
public record Payment(
    int Id,
    Account OriginatorAccount,
    Account BeneficiaryAccount,
    Money Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber
)
{
}
