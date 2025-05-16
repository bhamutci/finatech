namespace FinaTech.Core.Payment;

using System.ComponentModel.DataAnnotations;
using Account;


/// <summary>
/// Represents a payment transaction between two accounts.
/// </summary>
public sealed class Payment
{
    /// <summary>
    /// Gets or sets the unique identifier for a payment.
    /// This identifier is the primary key for the Payment entity in the database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the originator's account.
    /// This represents the account from which the payment is initiated.
    /// </summary>
    public int OriginatorAccountId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the beneficiary account associated with the payment.
    /// This value links the payment to the relevant beneficiary account in the system.
    /// </summary>
    public int BeneficiaryAccountId { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount associated with the payment,
    /// including the value and the currency in which the transaction is denominated.
    /// </summary>
    public required Money Amount { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the payment transaction.
    /// This represents when the payment was initiated or processed.
    /// </summary>
    public required DateTimeOffset Date { get; set; }

    /// <summary>
    /// Gets or sets the account of the originator associated with the payment.
    /// This property establishes a relationship to the <see cref="Account"/> entity,
    /// representing the sender's account in the transaction.
    /// </summary>
    public required Account OriginatorAccount { get; set; }

    /// <summary>
    /// Gets or sets the beneficiary account associated with the payment.
    /// This represents the account that will receive the payment amount.
    /// </summary>
    public required Account BeneficiaryAccount { get; set; }

    /// <summary>
    /// Gets or sets the charge bearer for the payment transaction.
    /// Indicates who is responsible for bearing the transaction charges (Originator, Beneficiary, or Shared).
    /// </summary>
    public required int ChargesBearer { get; set; }

    /// <summary>
    /// Gets or sets additional information or remarks about the payment.
    /// This property can store a description or any other textual details
    /// related to the payment process, with a maximum length defined by
    /// <see cref="PaymentConstants.MaxLengthOfDetails"/>.
    /// </summary>
    [StringLength(PaymentConstants.MaxLengthOfDetails)]
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the reference number associated with a payment transaction.
    /// This is a unique identifier used for tracking and referencing payments.
    /// The maximum length allowed for the reference number is defined in PaymentConstants.MaxLengthOfReferenceNumber.
    /// </summary>
    [StringLength(PaymentConstants.MaxLengthOfReferenceNumber)]
    public string? ReferenceNumber { get; set; }

}
