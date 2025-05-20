namespace FinaTech.Web.Blazor.Model.Validator;

using Model;
using FluentValidation;


public class MoneyValidator: AbstractValidator<MoneyModel>
{
    public MoneyValidator()
    {
        RuleFor(money => money.Value)
            .NotEmpty().WithMessage("Money value cannot be null or empty.");

        RuleFor(money => money.Currency)
            .NotEmpty().WithMessage("Money value cannot be null or empty.")
            .Length(3).WithMessage("Money currency must be 3 letters.");
    }
}
