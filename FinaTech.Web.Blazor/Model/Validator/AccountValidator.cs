namespace FinaTech.Web.Blazor.Model.Validator;

using FluentValidation;

using Model;

public class AccountValidator: AbstractValidator<AccountModel>
{
    public AccountValidator()
    {
        RuleFor(account => account.Name)
            .NotEmpty().WithMessage("Account name cannot be null or empty.");

        RuleFor(account => account.Iban)
            .NotEmpty().WithMessage("Account IBAN cannot be null or empty.");

        RuleFor(account => account.Bic)
            .NotEmpty().WithMessage("Account BIC cannot be null or empty.");

        RuleFor(account => account.AccountNumber)
            .NotEmpty().WithMessage("Account number cannot be null or empty.");

        RuleFor(account => account.AddressModel)
            .NotNull().WithMessage("Account address cannot be null.")
            .SetValidator(new AddressValidator());
    }
}
