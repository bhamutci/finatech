namespace FinaTech.Application.PaymentService.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a payment.
/// </summary>
public record PaymentDto(
    int Id,
    int OriginatorAccountId,
    int BeneficiaryAccountId,
    MoneyDto Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber
)
{
}
