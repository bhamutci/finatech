namespace FinaTech.Application.Services.Payment.Dto;

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
