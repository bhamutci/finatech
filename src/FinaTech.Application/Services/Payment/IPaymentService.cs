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
    /// <returns>A <see cref="PaymentDto"/> object representing the payment with the specified identifier.</returns>
    Task<PaymentDto> GetPaymentAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a collection of payments.
    /// </summary>
    /// <returns>A read-only list of <see cref="PaymentDto"/> objects representing the payments.</returns>
    Task<PagedResultDto<PaymentDto>> GetPaymentsAsync(PaymentFilter paymentFilter, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a new payment.
    /// </summary>
    /// <param name="payment">The <see cref="PaymentDto"/> object containing the details of the payment to create.</param>
    /// <returns>A <see cref="PaymentDto"/> object representing the newly created payment.</returns>
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto payment, CancellationToken cancellationToken);
}
