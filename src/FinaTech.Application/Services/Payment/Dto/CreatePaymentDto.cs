using FinaTech.Application.Services.Account.Dto;

namespace FinaTech.Application.Services.Payment.Dto;

public record CreatePaymentDto(
    CreateAccountDto? OriginatorAccount,
    CreateAccountDto? BeneficiaryAccount,
    MoneyDto? Amount,
    DateTimeOffset? Date,
    ChargesBearer? ChargesBearer,
    string? Details,
    string? ReferenceNumber);
