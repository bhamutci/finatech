namespace FinaTech.Core;

/// <summary>
/// Provides constants related to bank configurations and validations.
/// This class is designed to store static, read-only variables needed for consistent handling
/// of specific bank-related data fields such as bank name and Bank Identifier Code (BIC).
/// </summary>
public static class BankConstants
{
    /// <summary>
    /// Specifies the maximum allowed length of a bank name.
    /// Ensures that the length of the name does not exceed the defined limit for standardization and validation purposes.
    /// </summary>
    public const int MaxLengthOfName = 100;

    /// <summary>
    /// Defines the maximum allowed length of a Bank Identifier Code (BIC).
    /// Used to validate and ensure the BIC conforms to the standard length requirements.
    /// </summary>
    public const int MaxLengthOfBIC = 11;
}
