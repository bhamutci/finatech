namespace FinaTech.Application.Services.Payment.Dto;

public record ListPayment(
    int Id,
    string Originator,
    string Beneficiary,
    Money Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber);
