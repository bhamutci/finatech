namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents the data transfer object for creating a new account in the system.
/// </summary>
/// <param name="Name">The name associated with the account.</param>
/// <param name="Iban">The International Bank Account Number (IBAN) of the account.</param>
/// <param name="Bic">The Bank Identifier Code (BIC) of the account.</param>
/// <param name="AccountNumber">The local account number associated with the account.</param>
/// <param name="Address">The address information linked to the account.</param>
public record CreateAccount(
    string Name,
    string Iban,
    string Bic,
    string AccountNumber,
    CreateAddress Address)
{
}
