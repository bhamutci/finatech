namespace FinaTech.Application.PaymentService;

using AutoMapper;
using Dto;
using EntityFramework.PostgresSqlServer;
using Core;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service for handling operations related to payments in the FinaTech application.
/// </summary>
public class PaymentService : IPaymentService
{
    #region Fields

    /// <summary>
    /// Represents the database context used for interacting with the application's database.
    /// Provides access to database entities such as Payments.
    /// </summary>
    private readonly FinaTechPostgresSqlDbContext dbContext;

    /// <summary>
    /// Provides object-object mapping functionality within the payment service,
    /// allowing transformations between domain models and DTOs.
    /// </summary>
    private readonly IMapper mapper;

    #endregion

    #region Constructors

    /// <summary>
    /// Service for handling operations related to payments in the FinaTech application.
    /// </summary>
    public PaymentService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Retrieves a payment based on the provided ID.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to be retrieved.</param>
    /// <returns>A <see cref="PaymentDto"/> representing the details of the payment, or null if no payment is found.</returns>
    public async Task<PaymentDto> GetPaymentAsync(int id)
    {
        var payment = await dbContext.Payments.FindAsync(id);
        return mapper.Map<Payment, PaymentDto>(payment);
    }

    /// <summary>
    /// Asynchronously creates a new payment in the system and returns the created payment details.
    /// </summary>
    /// <param name="payment">The payment details to be created, encapsulated in a <see cref="PaymentDto"/> object.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with a result of <see cref="PaymentDto"/> containing the details of the newly created payment.</returns>
    public async Task<PaymentDto> CreatePaymentAsync(PaymentDto payment)
    {
        Payment paymentEntity =mapper.Map<PaymentDto, Payment>(payment);
        await dbContext.Payments.AddAsync(paymentEntity);
        await dbContext.SaveChangesAsync();

        return mapper.Map<Payment, PaymentDto>(paymentEntity);
    }

    /// <summary>
    /// Retrieves a list of all payments.
    /// </summary>
    /// <returns>A read-only list of <see cref="PaymentDto"/> objects representing payments.</returns>
    public async Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync()
    {
        IReadOnlyList<Payment> payments = await dbContext.Payments.ToListAsync();
        return mapper.Map<IReadOnlyList<Payment>, IReadOnlyList<PaymentDto>>(payments);
    }

    #endregion
}
