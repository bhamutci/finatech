namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Specifies the entity responsible for bearing the charges related to a payment transaction.
/// </summary>
public enum ChargesBearer
{
    Originator,
    Beneficiary,
    Shared
}
