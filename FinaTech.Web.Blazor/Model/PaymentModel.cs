namespace FinaTech.Web.Blazor.Model;

public class PaymentModel
{
    public AccountModel OriginatorAccountModel { get; set; } = new();
    public AccountModel BeneficiaryAccountModel { get; set; } = new();
    public MoneyModel Amount { get; set; } = new(0, string.Empty);

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public ChargesBearer ChargesBearer { get; set; } = ChargesBearer.Shared;
    public string Details { get; set; }= string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
}
