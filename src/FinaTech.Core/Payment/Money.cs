using System.ComponentModel.DataAnnotations.Schema;

namespace FinaTech.Core;

[ComplexType]
public record Money(
    decimal Value,
    string Currency);
