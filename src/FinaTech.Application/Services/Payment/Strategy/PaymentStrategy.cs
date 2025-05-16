namespace FinaTech.Application.Services.Strategy;

using Payment.Dto;

public abstract class PaymentStrategy(Payment payment)
{
    /// <summary>
    /// Represents a data transfer object encapsulating payment details used in payment processing strategies.
    /// This object carries information related to the payment transaction,
    /// such as involved parties, the amount, currency, date, and other associated metadata.
    /// </summary>
    protected Payment Payment = payment;

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
