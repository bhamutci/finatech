namespace FinaTech.Web.Blazor.Model.Validator;

using FluentValidation;

using Model;

public class AddressValidator:AbstractValidator<AddressModel>
{
    public AddressValidator()
    {
        RuleFor(address => address.AddressLine1)
            .NotEmpty().WithMessage("Address Line1 cannot be null or empty.");

        RuleFor(address => address.CountryCode)
            .NotEmpty().WithMessage("Address country code cannot be null or empty.")
            .Length(2).WithMessage("Address country code must be 2 letters.");
    }
}
