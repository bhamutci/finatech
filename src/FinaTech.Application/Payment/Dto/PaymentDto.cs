namespace FinaTech.Application.Payment.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a payment.
/// </summary>
public record PaymentDto(
    int Id,
    int OriginatorAccountId,
    int BeneficiaryAccountId,
    MoneyDto Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber
)
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="currency"></param>
    /// <param name="details"></param>
    /// <param name="reference"></param>
    /// <returns>Payment</returns>
    public static PaymentDto Create(int Id,
        int originatorAccountId,
        int beneficiaryAccountId,
        MoneyDto amount,
        DateTimeOffset date,
        ChargesBearer chargesBearer,
        string details,
        string reference)
    {
        ValidateInputs(amount, date, details);
        return new PaymentDto(Id,
            originatorAccountId,
            beneficiaryAccountId,
            amount,
            date,
            chargesBearer,
            details,
            reference);
    }

    /// <summary>
    /// Validates the provided input parameters to ensure they meet the required conditions.
    /// </summary>
    /// <param name="amount">The payment amount, cannot be null or empty.</param>
    /// <param name="currency">The currency of the payment, cannot be null or empty.</param>
    /// <param name="details">The details of the payment, cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when any input parameter is null or empty.</exception>
    private static void ValidateInputs(MoneyDto amount, DateTimeOffset date, string details)
    {
        if (date > DateTimeOffset.UtcNow)
            throw new ArgumentException("Date cannot be in the future", nameof(date));

        if (amount == null)
            throw new ArgumentException("Amount cannot be null or empty", nameof(amount));

        if (details == null)
            throw new ArgumentException("Details cannot be null or empty", nameof(details));
    }
}
