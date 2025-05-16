namespace FinaTech.Application.Services.Payment.Dto;

public record CreatePayment(
    CreateAccount OriginatorAccount,
    CreateAccount BeneficiaryAccount,
    Money Amount,
    DateTimeOffset? Date,
    ChargesBearer? ChargesBearer,
    string? Details,
    string? ReferenceNumber);
