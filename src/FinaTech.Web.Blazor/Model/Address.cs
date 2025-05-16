namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents an address entity with details including identification, address lines,
/// city, postal code, and country code.
/// </summary>
/// <param name="Id">The unique identifier for the address.</param>
/// <param name="AddressLine1">The primary line of the address.</param>
/// <param name="AddressLine2">The secondary line of the address, if applicable.</param>
/// <param name="AddressLine3">The tertiary line of the address, if applicable.</param>
/// <param name="City">The city associated with the address.</param>
/// <param name="PostCode">The postal code of the address, if applicable.</param>
/// <param name="CountryCode">The code representing the country of the address.</param>
public record Address(
    int Id,
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? City,
    string? PostCode,
    string CountryCode)
{
}
