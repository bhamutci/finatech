using FinaTech.Application.Services.Account;
using FinaTech.Application.Services.Account.Dto;
using FluentValidation;

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
    /// Validator for the <see cref="CreatePaymentDto"/> object, ensuring
    /// that the provided payment data is valid and conforms to predefined
    /// business and application rules.
    /// </summary>
    private readonly IValidator<CreatePaymentDto> paymentValidator;

    #endregion

    #region Constructors

    /// <summary>
    /// Service for handling operations related to payments in the FinaTech application.
    /// </summary>
    public PaymentService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper, ILogger<PaymentService> logger,
        IValidator<CreatePaymentDto> paymentValidator) : base(dbContext, mapper, logger)
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Retrieves a payment based on the provided ID.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to be retrieved.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="PaymentDto"/> representing the details of the payment, or null if no payment is found.</returns>
    public async Task<PaymentDto?> GetPaymentAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve payment with ID: {PaymentId}", id);

        try
        {
            var payment = await dbContext.Payments.FindAsync([id], cancellationToken: cancellationToken);

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
    public async Task<PagedResultDto<PaymentDto>> GetPaymentsAsync(PaymentFilter? paymentFilter, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve payments with filter: Keywords='{Keywords}', BeneficiaryAccountId='{BeneficiaryAccountId}', OriginatorAccountId='{OriginatorAccountId}', Skip={Skip}, Take={Take}",
            paymentFilter?.Keywords, paymentFilter?.BeneficiaryAccountId, paymentFilter?.OriginatorAccountId, paymentFilter?.SkipCount, paymentFilter?.MaxResultCount);

        try
        {
            ArgumentNullException.ThrowIfNull(paymentFilter);

            var paymentQuery = GetPaymentsQuery(paymentFilter, cancellationToken);

            var totalCount = await paymentQuery.CountAsync(cancellationToken);

            var payments = await paymentQuery
                .Skip(paymentFilter.SkipCount)
                .Take(paymentFilter.MaxResultCount)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var paymentDtos =
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
    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto? payment, CancellationToken cancellationToken)
    {
        try
        {
            return await SavePaymentAsync(payment, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreatePaymentAsync for ReferenceNumber {ReferenceNumber} was cancelled.", payment?.ReferenceNumber);
            throw;
        }
        catch (ValidationException valEx)
        {
            logger.LogDebug(valEx, "Payment creation aborted due to validation errors.");
            throw;
        }
        catch (ArgumentException argEx)
        {
            logger.LogWarning(argEx, "Payment creation failed due to invalid input: {Message}", argEx.Message);
            throw;
        }
        catch (AccountException ex)
        {
            logger.LogWarning(ex, "Payment creation failed due to invalid input: {Message}", ex.Message);
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
    /// Saves a payment asynchronously to the database and returns the saved payment details.
    /// </summary>
    /// <param name="payment">The payment data transfer object containing the details of the payment to be saved.</param>
    /// <param name="cancellationToken">The cancellation token to observe for request cancellation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the saved payment data transfer object.</returns>
    private async Task<PaymentDto> SavePaymentAsync(CreatePaymentDto? payment, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mapping PaymentDto to Payment entity for saving.");

        if (payment == null)
        {
            logger.LogWarning("Validation failed: Payment details cannot be null.");
            throw new ArgumentNullException(nameof(payment), "Payment details cannot be null.");
        }

        var validationResult = await ValidatePaymentAsync(payment, cancellationToken);

        if (validationResult.Count > 0)
        {
            throw new ValidationException("Payment input validation failed.", validationResult);
        }

        var beneficiaryAccount = await GetOrCreateAccountAsync(payment.BeneficiaryAccount, cancellationToken);

        var originatorAccount = await GetOrCreateAccountAsync(payment.OriginatorAccount, cancellationToken);

        var paymentEntity = mapper.Map<CreatePaymentDto, Payment>(payment);

        paymentEntity.BeneficiaryAccountId = beneficiaryAccount.Id;

        paymentEntity.OriginatorAccountId = originatorAccount.Id;

        await dbContext.Payments.AddAsync(paymentEntity, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(paymentEntity).ReloadAsync(cancellationToken);

        logger.LogInformation("Payment created successfully with ID: {PaymentId}", paymentEntity.Id);
        return mapper.Map<Payment, PaymentDto>(paymentEntity);
    }

    /// <summary>
    /// Constructs a queryable collection of payments based on the provided filter criteria.
    /// </summary>
    /// <param name="paymentFilter">The filter object containing criteria such as keywords, beneficiary account ID, or originator account ID.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A queryable collection of payments filtered based on the criteria specified within the paymentFilter.</returns>
    private IQueryable<Payment> GetPaymentsQuery(PaymentFilter? paymentFilter, CancellationToken cancellationToken)
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
    /// Retrieves an existing account by matching criteria or creates a new account if one does not exist.
    /// </summary>
    /// <param name="createAccount">The account details to search or create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the account details.</returns>
    private async Task<AccountDto> GetOrCreateAccountAsync(CreateAccountDto createAccount,
        CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.Where(a=>a.Name.Contains(createAccount.Name) ||
                                                        a.AccountNumber.Contains(createAccount.AccountNumber) ||
                                                        a.Iban.Contains(createAccount.Iban)).FirstOrDefaultAsync(cancellationToken: cancellationToken);;

        if (account == null)
        {
            account = mapper.Map<CreateAccountDto, Core.Account>(createAccount);
            await dbContext.Accounts.AddAsync(account, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Entry(account).ReloadAsync(cancellationToken);
        }

        return mapper.Map<Account, AccountDto>(account);
    }

    /// <summary>
    /// Validates provided payment information, ensuring all required fields meet the expected criteria.
    /// </summary>
    /// <param name="payment">The payment data to validate, including originator and beneficiary account details, amount, date, reference number, and other related information.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of validation error messages, if any, or an empty list if validation is successful.</returns>
    private async Task<IReadOnlyList<string>> ValidatePaymentAsync(CreatePaymentDto payment,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation("Starting validation for payment: {ReferenceNumber}", payment?.ReferenceNumber);

        var validationResult = await paymentValidator.ValidateAsync(payment, cancellationToken);

        var errors = validationResult.Errors.Select(failure => failure.ErrorMessage).ToList();

        if (errors.Count == 0)
        {
            logger.LogDebug("Payment validation completed successfully.");
        } else {
            logger.LogWarning("Payment validation completed with {ErrorCount} errors. Details: {Errors}", errors.Count, string.Join("; ", errors));
        }

        return errors.AsReadOnly();
    }

    #endregion
}
