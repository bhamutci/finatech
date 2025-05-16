namespace FinaTech.Web.Blazor.Model;

public record ListPayment(
    int Id,
    string Originator,
    string Beneficiary,
    Money Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber);
