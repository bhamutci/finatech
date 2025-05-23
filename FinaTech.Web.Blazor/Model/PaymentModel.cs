namespace FinaTech.Web.Blazor.Model;

/// <summary>
/// Represents a model for processing a payment transaction. Encapsulates the details related to the originator,
/// beneficiary, amount, charge bearer, and additional payment information.
/// </summary>
public class PaymentModel
{
    /// <summary>
    /// Gets or sets the account details of the originator involved in the payment transaction.
    /// This typically includes information such as IBAN, BIC, account number, name, and address.
    /// </summary>
    public AccountModel OriginatorAccount { get; set; } = new();

    /// <summary>
    /// Gets or sets the account details of the beneficiary involved in the payment transaction.
    /// This includes information such as account name, IBAN, BIC, account number, and associated address.
    /// </summary>
    public AccountModel BeneficiaryAccount { get; set; } = new();

    /// <summary>
    /// Gets or sets the payment amount for the transaction.
    /// This includes both the monetary value and its associated currency.
    /// </summary>
    public MoneyModel Amount { get; set; } = new(0, string.Empty);

    /// <summary>
    /// Gets or sets the date and time associated with the payment transaction.
    /// This typically represents the timestamp when the payment is created or processed.
    /// </summary>
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the party responsible for bearing the charges associated with the payment transaction.
    /// Possible values include Originator, Beneficiary, or Shared, representing who will cover the transaction fees.
    /// </summary>
    public ChargesBearer ChargesBearer { get; set; } = ChargesBearer.Shared;

    /// <summary>
    /// Gets or sets the additional details or description of the payment transaction.
    /// This property can include any relevant information or purpose related to the transaction.
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference number associated with the payment transaction.
    /// The reference number is a unique identifier used to track and differentiate
    /// specific payment transactions.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;
}
