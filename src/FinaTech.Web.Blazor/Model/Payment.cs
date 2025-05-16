namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents a payment transaction containing information about the originator,
/// beneficiary, transaction amount, date, and other related details.
/// </summary>
/// <param name="Id">The unique identifier for the payment.</param>
/// <param name="OriginatorAccount">The account entity of the originator responsible for initiating the payment.</param>
/// <param name="BeneficiaryAccount">The account entity of the beneficiary who will receive the payment.</param>
/// <param name="Amount">The monetary value and currency associated with the payment.</param>
/// <param name="Date">The date and time when the payment transaction is executed or scheduled.</param>
/// <param name="ChargesBearer">Specifies the entity responsible for bearing the charges incurred during the transaction.</param>
/// <param name="Details">Optional details or notes describing the payment.</param>
/// <param name="ReferenceNumber">Optional payment reference number for tracking or identification.</param>
public record Payment(
    int Id,
    Account OriginatorAccount,
    Account BeneficiaryAccount,
    Money Amount,
    DateTimeOffset Date,
    ChargesBearer ChargesBearer,
    string? Details,
    string? ReferenceNumber
)
{
}
