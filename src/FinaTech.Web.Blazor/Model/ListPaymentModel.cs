namespace FinaTech.Web.Blazor.Model;

public class ListPaymentModel
{
    public int Id { get; set; }
    public string Originator { get; set; }
    public string Beneficiary { get; set; }
    public MoneyModel Amount { get; set; }
    public DateTimeOffset Date { get; set; }
    public ChargesBearer ChargesBearer { get; set; }
    public string? Details { get; set; }
    public string? ReferenceNumber { get; set; }
}
