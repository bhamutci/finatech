namespace FinaTech.Application.Services.Dto;

/// <summary>
/// Defines a request interface for obtaining paginated results.
/// </summary>
public class IFilter
{
    /// <summary>
    /// Gets or sets the keywords used for searching or filtering data.
    /// This property is typically employed to refine the results based on specific terms or phrases.
    /// </summary>
    public string Keywords { get; set; }

    /// <summary>
    /// Gets or sets the number of items to skip before starting to retrieve results.
    /// This is typically used for pagination to specify the starting point of the result set.
    /// </summary>
    public int SkipCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items to retrieve in the result set.
    /// This property is commonly used to define the size of a page for paginated data.
    /// </summary>
    public int MaxResultCount { get; set; }
}
