namespace FinaTech.Application.Services.Account.Dto.Validator;

using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="AddressDto"/> object.
/// Ensures that the address fields adhere to defined constraints to maintain data consistency and validity.
/// </summary>
/// <remarks>
/// The validator includes:
/// - Ensuring the primary address line (<see cref="AddressDto.AddressLine1"/>) is not empty or null.
/// - Verifying that the country code (<see cref="AddressDto.CountryCode"/>) is a non-null, non-empty,
/// two-letter string adhering to standardized country code formats.
/// </remarks>
public class AddressDtoValidator: AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(address => address.AddressLine1)
            .NotEmpty().WithMessage("Address Line1 cannot be null or empty.");

        RuleFor(address => address.CountryCode)
            .NotEmpty().WithMessage("Address country code cannot be null or empty.")
            .Length(2).WithMessage("Address country code must be 2 letters.");
    }

}
