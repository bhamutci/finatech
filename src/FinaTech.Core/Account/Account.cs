namespace FinaTech.Core.Account;

using System.ComponentModel.DataAnnotations;

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
    /// Gets or sets the unique identifier for the associated address entity.
    /// </summary>
    [Required]
    public int AddressId { get; set; }

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
    [Required]
    [StringLength(AccountConstants.MaxLengthOfAccountNumber)]
    public required string AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the International Bank Account Number (IBAN) associated with the account.
    /// </summary>
    [Required]
    [StringLength(AccountConstants.MaxLengthOfIban)]
    public required string Iban { get; set; }

    /// <summary>
    /// Gets or sets the International Bank Account Number (IBAN) associated with the account.
    /// </summary>
    [Required]
    [StringLength(AccountConstants.MaxLengthOfBic)]
    public required string Bic { get; set; }

    /// <summary>
    /// Gets or sets the address associated with the account entity.
    /// </summary>
    public Address? Address { get; set; }

}

