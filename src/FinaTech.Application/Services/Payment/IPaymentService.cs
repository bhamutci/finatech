namespace FinaTech.Application.Services.Payment;

using Dto;
using FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a service for managing payment operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Asynchronously retrieves a payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to retrieve.</param>
    /// <returns>A <see cref="Payment"/> object representing the payment with the specified identifier.</returns>
    Task<Payment?> GetPaymentAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a collection of payments.
    /// </summary>
    /// <returns>A read-only list of <see cref="Payment"/> objects representing the payments.</returns>
    Task<PagedResultDto<Payment>> GetPaymentsAsync(PaymentFilter? paymentFilter, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a new payment.
    /// </summary>
    /// <param name="payment">The <see cref="Payment"/> object containing the details of the payment to create.</param>
    /// <returns>A <see cref="Payment"/> object representing the newly created payment.</returns>
    Task<Payment> CreatePaymentAsync(CreatePayment payment, CancellationToken cancellationToken);
}
