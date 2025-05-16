namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents an account entity with details including identifiers, account-specific information,
/// and an associated address.
/// </summary>
/// <param name="Id">The unique identifier of the account.</param>
/// <param name="AddressId">The unique identifier of the associated address.</param>
/// <param name="Name">The name of the account holder.</param>
/// <param name="Iban">The International Bank Account Number (IBAN) of the account.</param>
/// <param name="Bic">The Bank Identifier Code (BIC) of the account.</param>
/// <param name="AccountNumber">The account number, if available.</param>
/// <param name="Address">The associated address information, if provided.</param>
public record Account(
    int Id,
    int AddressId,
    string Name,
    string Iban,
    string Bic,
    string? AccountNumber,
    Address? Address)
{
}
