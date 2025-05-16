namespace FinaTech.Application.Services.Payment.Dto.Validator;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="Money"/> class.
/// Ensures that the MoneyDto fields meet the defined validation rules.
/// </summary>
public class MoneyDtoValidator: AbstractValidator<Money>
{
    public MoneyDtoValidator()
    {
        RuleFor(money => money.Value)
            .NotEmpty().WithMessage("Money value cannot be null or empty.");

        RuleFor(money => money.Currency)
            .NotEmpty().WithMessage("Money value cannot be null or empty.")
            .Length(3).WithMessage("Money currency must be 3 letters.");

    }
}
