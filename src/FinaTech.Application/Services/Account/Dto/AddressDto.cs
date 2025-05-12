namespace FinaTech.Application.Services.Account.Dto;

public record AddressDto(string AddressLine1, string? AddressLine2, string? AddressLine3, string? City, string? PostalCode, string Country)
{ }
