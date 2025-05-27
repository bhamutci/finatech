namespace FinaTech.Web.Blazor.Model.Validator;

using Model;
using FluentValidation;


public class PaymentValidator: AbstractValidator<PaymentModel>
{
    public PaymentValidator()
    {
        RuleFor(payment => payment.ReferenceNumber)
            .NotEmpty()
            .WithMessage("Payment reference number cannot be null or empty.");

        RuleFor(payment => payment.Details)
            .NotEmpty()
            .WithMessage("Payment details cannot be null or empty.");

        RuleFor(payment => payment.Amount)
            .NotNull().WithMessage("Payment amount cannot be null.")
            .SetValidator(new MoneyValidator());

        RuleFor(payment => payment.BeneficiaryAccount)
            .NotNull().WithMessage("Payment beneficiary account cannot be null.")
            .SetValidator(new AccountValidator());

        RuleFor(payment => payment.OriginatorAccount)
            .NotNull().WithMessage("Payment originator account cannot be null.")
            .SetValidator(new AccountValidator());

    }
}
