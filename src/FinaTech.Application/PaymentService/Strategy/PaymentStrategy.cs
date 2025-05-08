using FinaTech.Application.PaymentService.Dto;

namespace FinaTech.Application.PaymentService;

public abstract class PaymentStrategy(PaymentDto payment)
{
    protected PaymentDto paymentDto = payment;

    /// <summary>
    /// Executes the core logic for processing a payment strategy, allowing
    /// derived classes to define specific implementation details.
    /// </summary>
    protected abstract void Process();

    /// <summary>
    /// Validates the payment strategy implementation to ensure
    /// it meets specific criteria or rules defined in the derived class.
    /// </summary>
    /// <returns>Returns true if the validation is successful; otherwise, false.</returns>
    protected abstract bool Validate();
}
