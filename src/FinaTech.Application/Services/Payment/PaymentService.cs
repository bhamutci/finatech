namespace FinaTech.Application.Services.Payment;

using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

using Core;
using Dto;
using Exceptions;
using EntityFramework.PostgresSqlServer;
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
    public PaymentService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper, ILogger<PaymentService> logger): base(dbContext, mapper, logger)
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
        logger.LogInformation("Attempting to retrieve payment with ID: {PaymentId}", id);

        try
        {
            var payment = await dbContext.Payments.FindAsync([id], cancellationToken: cancellationToken);;

            if (payment == null)
            {
                logger.LogWarning("Payment with ID: {PaymentId} not found.", id);

                return null;
            }

            return mapper.Map<Payment, PaymentDto>(payment);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetPaymentAsync for ID {PaymentId} was cancelled.", id);
            throw; // Re-throw the cancellation exception
        }
        catch (DbException dbEx) // Catch specific database exceptions
        {
            logger.LogError(dbEx, "Database error while retrieving payment with ID: {PaymentId}", id); // Log database error
            throw new PaymentException($"Could not retrieve payment with ID {id} due to a database error.", dbEx); // Throw custom exception
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving payment with ID: {PaymentId}", id);
            throw new PaymentException($"An unexpected error occurred while retrieving payment with ID {id}.", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of all payments.
    /// </summary>
    /// <returns>A read-only list of <see cref="PaymentDto"/> objects representing payments.</returns>
    public async Task<PagedResultDto<PaymentDto>> GetPaymentsAsync(PaymentFilter paymentFilter, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve payments with filter: Keywords='{Keywords}', BeneficiaryAccountId='{BeneficiaryAccountId}', OriginatorAccountId='{OriginatorAccountId}', Skip={Skip}, Take={Take}",
            paymentFilter?.Keywords, paymentFilter?.BeneficiaryAccountId, paymentFilter?.OriginatorAccountId, paymentFilter?.SkipCount, paymentFilter?.MaxResultCount);

        try
        {
            IQueryable<Payment> paymentQuery = GetPaymentsQuery(paymentFilter, cancellationToken);

            int totalCount = await paymentQuery.CountAsync(cancellationToken);

            List<Payment> payments = await paymentQuery.Skip(paymentFilter.SkipCount).Take(paymentFilter.MaxResultCount)
                .ToListAsync(cancellationToken);

            IReadOnlyList<PaymentDto> paymentDtos =
                mapper.Map<IReadOnlyList<Payment>, IReadOnlyList<PaymentDto>>(payments);

            var pagedResultDto = new PagedResultDto<PaymentDto>
            {
                Items = paymentDtos,
                TotalCount = totalCount
            };

            logger.LogInformation("Retrieved {PaymentCount} payments out of {TotalCount} total payments.",
                paymentDtos.Count, totalCount);

            return pagedResultDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("GetPaymentsAsync was cancelled with filter: {Filter}", paymentFilter);
            throw;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while retrieving payments with filter: {Filter}", paymentFilter);
            throw new PaymentException("Could not retrieve payments due to a database error.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while retrieving payments with filter: {Filter}", paymentFilter);
            throw new PaymentException("An unexpected error occurred while retrieving payments.", ex);
        }
    }

    /// <summary>
    /// Asynchronously creates a new payment.
    /// </summary>
    /// <param name="payment">The details of the payment to be created.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="PaymentDto"/> object representing the created payment.</returns>
    /// <exception cref="PaymentException">Thrown when there is an issue creating the payment.</exception>
    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto payment, CancellationToken cancellationToken)
    {
        try
        {
            ValidatePayment(payment, cancellationToken);

            var paymentDto = await SavePaymentAsync(payment, cancellationToken);

            logger.LogInformation("Payment created successfully with ID: {PaymentId}", paymentDto.Id);
            return paymentDto;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreatePaymentAsync for ReferenceNumber {ReferenceNumber} was cancelled.", payment?.ReferenceNumber);
            throw;
        }
        catch (ArgumentException argEx)
        {
            logger.LogWarning(argEx, "Payment creation failed due to invalid input: {Message}", argEx.Message);
            throw;
        }
        catch (PaymentException)
        {
             throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while creating a payment. ReferenceNumber: {ReferenceNumber}", payment?.ReferenceNumber);
            throw new PaymentException("An unexpected error occurred while creating the payment.", ex);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Asynchronously saves a payment transaction to the database.
    /// </summary>
    /// <param name="payment">The payment data to be saved, represented as a <see cref="CreatePaymentDto"/> object.</param>
    /// <param name="cancellationToken">
    /// A token used to monitor for cancellation requests, allowing the operation to be canceled if necessary.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains the information about the saved payment represented as a <see cref="PaymentDto"/> object.
    /// </returns>
    /// <exception cref="PaymentAlreadyExistsException">
    /// Thrown when a payment with the same unique identifier already exists in the database.
    /// </exception>
    /// <exception cref="PaymentException">
    /// Thrown when an unexpected error occurs while processing or saving the payment.
    /// </exception>
    private async Task<PaymentDto> SavePaymentAsync(CreatePaymentDto payment, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mapping PaymentDto to Payment entity for saving.");
        try
        {
            Payment paymentEntity = mapper.Map<CreatePaymentDto, Payment>(payment);
            await dbContext.Payments.AddAsync(paymentEntity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await dbContext.Entry(paymentEntity).ReloadAsync(cancellationToken);

            return mapper.Map<Payment, PaymentDto>(paymentEntity);

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("SavePaymentAsync for ReferenceNumber {ReferenceNumber} was cancelled.", payment?.ReferenceNumber);
            throw;
        }
        catch (DbUpdateException dbUpdateEx)
        {
            logger.LogError(dbUpdateEx, "Database update error while saving payment with ReferenceNumber: {ReferenceNumber}", payment?.ReferenceNumber);

            if (dbUpdateEx.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new PaymentAlreadyExistsException("A payment with this reference number already exists.", dbUpdateEx);
            }

            throw new PaymentException($"Could not save payment with ReferenceNumber {payment?.ReferenceNumber} due to a database update error.", dbUpdateEx);
        }
        catch (DbException dbEx) // Catch other general database errors (connection, etc.)
        {
            logger.LogError(dbEx, "A general database error occurred while saving payment with ReferenceNumber: {ReferenceNumber}", payment?.ReferenceNumber);
            throw new PaymentException($"A database error occurred while saving payment with ReferenceNumber {payment?.ReferenceNumber}.", dbEx);
        }
        catch (Exception ex) // Catch any other unexpected errors during save
        {
            logger.LogError(ex, "An unexpected error occurred while saving payment with ReferenceNumber: {ReferenceNumber}", payment?.ReferenceNumber);
            throw new PaymentException($"An unexpected error occurred while saving payment with ReferenceNumber {payment?.ReferenceNumber}.", ex);
        }
    }

    /// <summary>
    /// Constructs a queryable collection of payments based on the provided filter criteria.
    /// </summary>
    /// <param name="paymentFilter">The filter object containing criteria such as keywords, beneficiary account ID, or originator account ID.</param>
    /// <returns>A queryable collection of payments filtered based on the criteria specified within the paymentFilter.</returns>
    private IQueryable<Payment> GetPaymentsQuery(PaymentFilter paymentFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogDebug("Building payment query with filter: {Filter}", paymentFilter);

        var payments = dbContext.Payments
            .Include(p => p.BeneficiaryAccount)
            .Include(p => p.OriginatorAccount)
            .AsQueryable();

        if (paymentFilter != null)
        {
            if (!string.IsNullOrEmpty(paymentFilter.Keywords))
            {
                logger.LogDebug("Applying keyword filter: {Keywords}", paymentFilter.Keywords);
                payments = payments.Where(p => p.Details!=null && p.Details.Contains(paymentFilter.Keywords) ||
                                               p.ReferenceNumber!=null && p.ReferenceNumber.Contains(paymentFilter.Keywords));
            }

            if (paymentFilter.BeneficiaryAccountId.HasValue)
            {
                logger.LogDebug("Applying BeneficiaryAccountId filter: {BeneficiaryAccountId}", paymentFilter.BeneficiaryAccountId);
                payments = payments.Where(p => p.BeneficiaryAccountId == paymentFilter.BeneficiaryAccountId);
            }

            if (paymentFilter.OriginatorAccountId.HasValue)
            {
                logger.LogDebug("Applying OriginatorAccountId filter: {OriginatorAccountId}", paymentFilter.OriginatorAccountId);
                payments = payments.Where(p => p.OriginatorAccountId == paymentFilter.OriginatorAccountId);
            }
        }

        return payments;
    }

    /// <summary>
    /// Validates the provided payment details to ensure they meet the required criteria.
    /// </summary>
    /// <param name="payment">The payment data to be validated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when the payment or any required property of the payment is null or empty.</exception>
    private void ValidatePayment(CreatePaymentDto payment, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation("Starting validation for payment: {ReferenceNumber}", payment?.ReferenceNumber);

        if (payment == null)
        {
            logger.LogWarning("Validation failed: Payment details cannot be null.");
            throw new ArgumentNullException(nameof(payment), "Payment details cannot be null.");
        }

        if (string.IsNullOrEmpty(payment.ReferenceNumber))
        {
            logger.LogWarning("Validation failed: Payment reference number cannot be null or empty.");
            throw new ArgumentNullException(nameof(payment.ReferenceNumber), "Payment reference number cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(payment.Details))
        {
            logger.LogWarning("Validation failed: Payment details cannot be null or empty.");
            throw new ArgumentNullException(nameof(payment.Details), "Payment details cannot be null or empty.");
        }

        logger.LogDebug("Payment validation completed successfully.");

    }

    #endregion
}
