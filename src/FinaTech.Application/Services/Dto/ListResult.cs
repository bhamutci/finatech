namespace FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a result containing a list of items.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class ListResult<T> : IListResult<T>
{
    public IReadOnlyList<T> Items { get; set; }


    /// <summary>
    /// Creates a new <see cref="ListResult{T}"/> object.
    /// </summary>
    /// <param name="items">List of items</param>
    protected ListResult(IReadOnlyList<T> items)
    {
        Items = items;
    }
}
