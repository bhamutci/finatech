namespace FinaTech.Application.Services.Payment.Dto;

using FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a filter to be used for querying payments with specific constraints
/// such as beneficiary and originator account identifiers, as well as keywords,
/// skip count, and maximum result count via the ILimitedResult interface.
/// </summary>
public class PaymentFilter : Filter
{
    /// <summary>
    /// Gets or sets the identifier for the beneficiary's account.
    /// </summary>
    /// <remarks>
    /// This property represents the unique account ID for the party that receives the transaction or payment.
    /// </remarks>
    public int? BeneficiaryAccountId { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the originator's account.
    /// </summary>
    /// <remarks>
    /// This property represents the unique account ID for the party that initiates the transaction or payment.
    /// </remarks>
    public int? OriginatorAccountId { get; set; }
}
