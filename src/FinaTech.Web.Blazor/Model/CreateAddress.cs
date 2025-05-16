namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents the data transfer object for creating a new address.
/// </summary>
/// <param name="AddressLine1">The primary address line.</param>
/// <param name="AddressLine2">The second address line, if applicable.</param>
/// <param name="AddressLine3">The third address line, if applicable.</param>
/// <param name="City">The city of the address.</param>
/// <param name="PostCode">The postal code of the address.</param>
/// <param name="CountryCode">The ISO 3166-1 alpha-2 country code.</param>
public record CreateAddress(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? City,
    string? PostCode,
    string CountryCode)
{
}
