namespace FinaTech.Application.Services.Payment.Dto;

/// <summary>
/// Represents the data transfer object for creating an address.
/// </summary>
public record CreateAddress(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? City,
    string? PostCode,
    string CountryCode)
{
}
