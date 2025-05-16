namespace FinaTech.Web.Blazor.Model;

public record CreatePayment(
    CreateAccount OriginatorAccount,
    CreateAccount BeneficiaryAccount,
    Money Amount,
    DateTimeOffset? Date,
    ChargesBearer? ChargesBearer,
    string? Details,
    string? ReferenceNumber);
