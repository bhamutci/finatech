namespace FinaTech.Web.Blazor.Model;

public class AccountModel
{
    public string Name { get; set; }= string.Empty;
    public string Iban { get; set; }= string.Empty;
    public string Bic { get; set; }= string.Empty;
    public string AccountNumber { get; set; }= string.Empty;
    public AddressModel AddressModel { get; set; } = new();
}
