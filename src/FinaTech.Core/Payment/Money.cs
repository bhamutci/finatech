namespace FinaTech.Core.Payment;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents an amount of money and its associated currency.
/// </summary>
[ComplexType]
public record Money(
    decimal Value,
    string Currency);
