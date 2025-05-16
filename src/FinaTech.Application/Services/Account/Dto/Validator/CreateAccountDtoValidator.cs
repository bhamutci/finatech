namespace FinaTech.Application.Services.Account.Dto.Validator;

using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="CreateAccountDto"/> object.
/// Ensures that the account's required fields adhere to defined constraints to maintain data integrity.
/// </summary>
/// <remarks>
/// The validator includes:
/// - Ensuring the account name, IBAN, and BIC are non-empty and not null.
/// - Verifying that the account number, if present, is not empty or null.
/// - Validating the account address is non-null and conforms to the validation rules defined in <see cref="AddressDtoValidator"/>.
/// </remarks>
public class CreateAccountDtoValidator: AbstractValidator<CreateAccountDto>
{
    public CreateAccountDtoValidator()
    {
        RuleFor(account => account.Name)
            .NotEmpty().WithMessage("Account name cannot be null or empty.");

        RuleFor(account => account.Iban)
            .NotEmpty().WithMessage("Account IBAN cannot be null or empty.");

        RuleFor(account => account.Bic)
            .NotEmpty().WithMessage("Account BIC cannot be null or empty.");

        RuleFor(account => account.AccountNumber)
            .NotEmpty().WithMessage("Account number cannot be null or empty.");

        RuleFor(account => account.Address)
            .NotNull().WithMessage("Account address cannot be null.")
            .SetValidator(new AddressDtoValidator());
    }

}
