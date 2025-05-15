using System.ComponentModel.DataAnnotations;

namespace FinaTech.Core;

/// <summary>
/// Represents a physical or mailing address, encapsulating details such as address lines,
/// city, postal code, and country code. This class ensures validation rules are applied
/// to ensure data integrity, such as constraints on required fields and maximum length
/// for each property.
/// </summary>
public sealed class Address
{
    /// <summary>
    /// Gets or sets the unique identifier for the address entity.
    /// This serves as the primary key within its respective context.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the first line of the address, usually representing the primary details
    /// such as the street name and number. This property is required and has a maximum length constraint
    /// to ensure data consistency.
    /// </summary>
    [Required]
    [StringLength(AddressConstants.MaxLengthOfAddressLine1)]
    public required string AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the secondary address line, typically used for additional location details such as apartment or suite numbers.
    /// This field is optional and has a maximum allowable length defined by the associated AddressConstants.
    /// </summary>
    [StringLength(AddressConstants.MaxLengthOfAddressLine2)]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets an optional third line of the address.
    /// This property is typically used for additional address information
    /// such as building names, apartment numbers, or other secondary details.
    /// </summary>
    [StringLength(AddressConstants.MaxLengthOfAddressLine3)]
    public string? AddressLine3 { get; set; }

    /// <summary>
    /// Gets or sets the name of the city associated with the address.
    /// The value is optional and subject to a maximum length constraint as defined in <see cref="AddressConstants.MaxLengthOfCity"/>.
    /// </summary>
    [StringLength(AddressConstants.MaxLengthOfCity)]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the postal code associated with the address.
    /// This property can store up to a maximum number of characters
    /// as defined by the relevant constants to ensure validation.
    /// </summary>
    [StringLength(AddressConstants.MaxLengthOfPostCode)]
    public string? PostCode { get; set; }

    /// <summary>
    /// Gets or sets the ISO 3166-1 alpha-2 country code.
    /// This property is required and has a maximum length of 2 characters, ensuring valid country identification.
    /// </summary>
    [Required]
    [StringLength(AddressConstants.MaxLengthOfCountryCode)]
    public required string CountryCode { get; set; }

    /// <summary>
    /// Represents the address of an entity, including various address lines, city, postal code, and country code.
    /// </summary>
    /// <remarks>
    /// Useful for storing and validating address details in the application.
    /// Includes validation constraints for required fields and maximum field lengths.
    /// </remarks>
    public Address()
    {
        // Default parameterless constructor required for EF Core
    }
}
