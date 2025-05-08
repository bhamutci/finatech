using System.ComponentModel.DataAnnotations;

namespace FinaTech.Core;

/// <summary>
/// Represents a financial institution, providing details such as
/// the bank's ID, name, and its Business Identifier Code (BIC).
/// </summary>
public sealed class Bank
{
    /// <summary>
    /// Gets or sets the unique identifier for the bank.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the bank.
    /// </summary>
    [Required]
    [StringLength(BankConstants.MaxLengthOfName)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Business Identifier Code (BIC) of the bank,
    /// used to uniquely identify the financial institution in international transactions.
    /// </summary>
    [Required]
    [StringLength(BankConstants.MaxLengthOfBIC)]
    public string BIC { get; set; }

    /// <summary>
    /// Gets or sets the collection of accounts associated with the bank.
    /// </summary>
    public ICollection<Account> Accounts { get; set; }

}
