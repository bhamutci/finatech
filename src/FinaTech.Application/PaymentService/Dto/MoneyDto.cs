namespace FinaTech.Application.PaymentService.Dto;

public record MoneyDto(
    decimal Amount,
    string Currency);
