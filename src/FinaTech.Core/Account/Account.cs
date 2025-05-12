using System.ComponentModel.DataAnnotations;

namespace FinaTech.Core;

/// <summary>
/// Represents an account entity, providing details about the account such as name,
/// account number, IBAN, BIC, and associated address.
/// </summary>
public sealed class Account
{
    /// <summary>
    /// Gets or sets the unique identifier for the account entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the associated bank entity.
    /// </summary>
    [Required]
    public required int BankId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated address entity.
    /// </summary>
    [Required]
    public required int AddressId { get; set; }

    /// <summary>
    /// Gets or sets the name of the account.
    /// This property is required and has a maximum allowed length defined by <see cref="AccountConstants.MaxLengthOfName"/>.
    /// </summary>
    [Required]
    [StringLength(AccountConstants.MaxLengthOfName)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the account number associated with the account entity.
    /// </summary>
    [StringLength(AccountConstants.MaxLengthOfAccountNumber)]
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the International Bank Account Number (IBAN) associated with the account.
    /// </summary>
    [StringLength(AccountConstants.MaxLengthOfIban)]
    public required string Iban { get; set; }

    /// <summary>
    /// Gets or sets the address associated with the account entity.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Represents a financial institution associated with an account,
    /// detailing properties such as the bank's name and BIC.
    /// </summary>
    public Bank? Bank { get; set; }

}

