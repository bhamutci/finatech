namespace FinaTech.Core.Account;

/// <summary>
/// Defines constant values related to account properties, including limits for name,
/// account number, IBAN, and BIC lengths.
/// </summary>
public static class AccountConstants
{
    /// <summary>
    /// Specifies the maximum length allowed for the name of an account.
    /// This is used to validate the length of the Name property in the <see cref="Account"/> class.
    /// </summary>
    public const int MaxLengthOfName = 50;

    /// <summary>
    /// Specifies the maximum length allowed for the account number of an account.
    /// This is utilized to validate the length of the AccountNumber property in the <see cref="Account"/> class.
    /// </summary>
    public const int MaxLengthOfAccountNumber = 11;

    /// <summary>
    /// Specifies the maximum length allowed for the IBAN of an account.
    /// This is used to validate the length of the IBAN property in the <see cref="Account"/> class.
    /// </summary>
    public const int MaxLengthOfIban = 34;

    /// <summary>
    /// Specifies the maximum length allowed for the BIC (Bank Identifier Code) of an account.
    /// This is used to validate the length of the BIC property in the <see cref="Account"/> class.
    /// </summary>
    public const int MaxLengthOfBic = 11;
}
