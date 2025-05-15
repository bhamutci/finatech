namespace FinaTech.Application.Services.Payment.Dto;

public record CreatePaymentDto(
    int OriginatorAccountId,
    int BeneficiaryAccountId,
    MoneyDto Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber);
