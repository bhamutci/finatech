namespace FinaTech.Core;

public static class PaymentConstants
{
    /// <summary>
    /// Specifies the maximum allowed length for a reference number in a payment transaction.
    /// </summary>
    public const int MaxLengthOfReferenceNumber = 50;

    /// <summary>
    /// Specifies the maximum allowed length for the details or remarks associated with a payment transaction.
    /// </summary>
    public const int MaxLengthOfDetails = 100;

    /// <summary>
    /// Defines the maximum allowed length for a currency code in payment-related operations.
    /// </summary>
    public const int MaxLengthOfCurrency = 3;
}
