namespace FinaTech.Web.Blazor.Model;

public class MoneyModel
{
    public decimal Value { get; set; } = 0;
    public string Currency { get; set; }

    public MoneyModel(){}
    public MoneyModel(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public MoneyModel(string currency)
    {
        Currency = currency;
    }

    public override string ToString()
    {
        return $"{Value:C} {Currency}";
    }
}
