namespace FinaTech.Application.Services.Payment.Dto.Validator;

using FluentValidation;
using FinaTech.Application.Services.Account.Dto.Validator;

/// <summary>
/// Provides validation rules for the <see cref="CreatePaymentDto"/> object.
/// Ensures required fields are present and meet specific criteria such as format and value constraints.
/// </summary>
/// <remarks>
/// The validation includes:
/// - Checking that the reference number and payment details are not empty.
/// - Validating the payment amount is greater than zero and the currency is a valid 3-letter ISO 4217 currency code.
/// - Ensuring both originator and beneficiary accounts are non-null and valid using the <see cref="CreateAccountDtoValidator"/>.
/// </remarks>
public class CreatePaymentDtoValidator: AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(payment => payment.ReferenceNumber)
            .NotEmpty()
            .WithMessage("Payment reference number cannot be null or empty.");

        RuleFor(payment => payment.Details)
            .NotEmpty()
            .WithMessage("Payment details cannot be null or empty.");

        RuleFor(payment => payment.Amount.Value)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero.");

         RuleFor(payment => payment.Amount.Currency)
             .NotEmpty().WithMessage("Payment currency cannot be null or empty.")
             .Length(3).WithMessage("Payment currency must be a 3-letter code.");

        RuleFor(payment => payment.BeneficiaryAccount)
            .NotNull().WithMessage("Payment beneficiary account cannot be null.")
            .SetValidator(new CreateAccountDtoValidator());

        RuleFor(payment => payment.OriginatorAccount)
            .NotNull().WithMessage("Payment originator account cannot be null.")
            .SetValidator(new CreateAccountDtoValidator());

    }
}
