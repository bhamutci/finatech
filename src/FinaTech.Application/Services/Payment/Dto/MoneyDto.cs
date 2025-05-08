namespace FinaTech.Application.Services.Payment.Dto;

public record MoneyDto(
    decimal Amount,
    string Currency);
