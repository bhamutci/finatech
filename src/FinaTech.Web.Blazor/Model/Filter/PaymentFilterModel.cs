namespace FinaTech.Web.Blazor.Model.Filter;

public class PaymentFilterModel
{
    /// <summary>
    /// Gets or sets the keywords used for searching or filtering data.
    /// This property is typically employed to refine the results based on specific terms or phrases.
    /// </summary>
    public string? Keywords { get; set; }

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

    /// <summary>
    /// Represents a filter model for retrieving payment-related data.
    /// This model is used to specify filter criteria such as keywords,
    /// the number of items to skip, and the maximum number of results to return.
    /// </summary>
    public PaymentFilterModel(string? keywords, int skipCount, int maxResultCount)
    {
        Keywords = keywords;
        SkipCount = skipCount;
        MaxResultCount = maxResultCount;
    }
}
