namespace FinaTech.Application.Services.Payment;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EntityFramework.PostgresSqlServer;
using Core;
using Dto;
using FinaTech.Application.Services.Dto;

/// <summary>
/// Service for handling operations related to payments in the FinaTech application.
/// </summary>
public class PaymentService : BaseApplicationService, IPaymentService
{
    #region Constructors

    /// <summary>
    /// Service for handling operations related to payments in the FinaTech application.
    /// </summary>
    public PaymentService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper): base(dbContext, mapper)
    {}

    #endregion

    #region Public Methods

    /// <summary>
    /// Retrieves a payment based on the provided ID.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to be retrieved.</param>
    /// <returns>A <see cref="PaymentDto"/> representing the details of the payment, or null if no payment is found.</returns>
    public async Task<PaymentDto> GetPaymentAsync(int id, CancellationToken cancellationToken)
    {
        var payment = await dbContext.Payments.FindAsync([id], cancellationToken: cancellationToken);;

        if (payment == null) return null;

        return mapper.Map<Payment, PaymentDto>(payment);
    }

    /// <summary>
    /// Retrieves a list of all payments.
    /// </summary>
    /// <returns>A read-only list of <see cref="PaymentDto"/> objects representing payments.</returns>
    public async Task<PagedResultDto<PaymentDto>> GetPaymentsAsync(PaymentFilter paymentFilter, CancellationToken cancellationToken)
    {
        var paymentQuery = GetPaymentsQuery(paymentFilter);
        var totalCount = await paymentQuery.CountAsync(cancellationToken);
        var payments = await paymentQuery.
            Skip(paymentFilter.SkipCount).
            Take(paymentFilter.MaxResultCount).
            ToListAsync(cancellationToken);

        var paymentDtos = mapper.Map<IReadOnlyList<Payment>, IReadOnlyList<PaymentDto>>(payments);

        var pagedResultDto = new PagedResultDto<PaymentDto>
        {
            Items = paymentDtos,
            TotalCount = totalCount
        };

        return pagedResultDto;
    }

    /// <summary>
    /// Asynchronously creates a new payment in the system and returns the created payment details.
    /// </summary>
    /// <param name="payment">The payment details to be created, encapsulated in a <see cref="PaymentDto"/> object.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with a result of <see cref="PaymentDto"/> containing the details of the newly created payment.</returns>
    public async Task<PaymentDto> CreatePaymentAsync(PaymentDto payment, CancellationToken cancellationToken)
    {
        await ValidatePaymentAsync(payment, cancellationToken);
        var paymentDto = await SavePaymentAsync(payment, cancellationToken);
        return paymentDto;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Asynchronously saves a payment to the database.
    /// </summary>
    /// <param name="payment">The payment data to be saved.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <return>A task representing the asynchronous operation, containing the saved payment as a <see cref="PaymentDto"/> object.</return>
    private async Task<PaymentDto> SavePaymentAsync(PaymentDto payment, CancellationToken cancellationToken)
    {
        Payment paymentEntity = mapper.Map<PaymentDto, Payment>(payment);
        await dbContext.Payments.AddAsync(paymentEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<Payment, PaymentDto>(paymentEntity);
    }

    /// <summary>
    /// Constructs a queryable collection of payments based on the provided filter criteria.
    /// </summary>
    /// <param name="paymentFilter">The filter object containing criteria such as keywords, beneficiary account ID, or originator account ID.</param>
    /// <returns>A queryable collection of payments filtered based on the criteria specified within the paymentFilter.</returns>
    private IQueryable<Payment> GetPaymentsQuery(PaymentFilter paymentFilter)
    {
        var payments = dbContext.Payments
            .Include(p => p.BeneficiaryAccount)
            .Include(p => p.OriginatorAccount)
            .AsQueryable();

        if (paymentFilter != null)
        {
            if (!string.IsNullOrEmpty(paymentFilter.Keywords))
            {
                payments = payments.Where(p => p.Details!=null && p.Details.Contains(paymentFilter.Keywords) ||
                                               p.ReferenceNumber!=null && p.ReferenceNumber.Contains(paymentFilter.Keywords));
            }

            if (paymentFilter.BeneficiaryAccountId.HasValue)
            {
                payments = payments.Where(p => p.BeneficiaryAccountId == paymentFilter.BeneficiaryAccountId);
            }

            if (paymentFilter.OriginatorAccountId.HasValue)
            {
                payments = payments.Where(p => p.OriginatorAccountId == paymentFilter.OriginatorAccountId);
            }
        }

        return payments;
    }

    /// <summary>
    /// Validates the specified payment details to ensure all required fields are correctly populated.
    /// </summary>
    /// <param name="payment">The payment details to validate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the payment, payment reference number, or payment details are null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the payment ID is specified in a new payment.
    /// </exception>
    private Task ValidatePaymentAsync(PaymentDto payment, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment), "Payment details cannot be null.");
        }

        if (string.IsNullOrEmpty(payment.ReferenceNumber))
        {
            throw new ArgumentNullException(nameof(payment.ReferenceNumber), "Payment reference number cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(payment.Details))
        {
            throw new ArgumentNullException(nameof(payment.Details), "Payment details cannot be null or empty.");
        }

        if (payment.Id != null && payment.Id != 0)
        {
           throw new ArgumentException("Payment ID cannot be specified when creating a new payment.", nameof(payment.Id));
        }

        return Task.CompletedTask;
    }

    #endregion
}
