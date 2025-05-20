namespace FinaTech.Web.Blazor.Model;

public class MoneyModel
{
    public decimal Value { get; set; } = 0;
    public string Currency { get; set; }

    public MoneyModel(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public MoneyModel()
    {

    }
}
