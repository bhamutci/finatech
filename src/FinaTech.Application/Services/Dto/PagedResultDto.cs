namespace FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a paged result of a query, containing a list of items and the total count of all items that satisfy the query conditions.
/// </summary>
/// <typeparam name="T">The type of the items in the result set.</typeparam>
public class PagedResultDto<T> : ListResultDto<T>, IPageResult<T>
{
  /// <summary>
  /// Gets or sets the total number of items that satisfy the query conditions.
  /// </summary>
  /// <remarks>
  /// This property represents the total count of items available for the query,
  /// regardless of the current page or the number of items contained in the result set.
  /// </remarks>
  public int TotalCount { get; set; }

  /// <summary>
  /// Represents a paged result containing a collection of items and a total count of all items that satisfy the query conditions.
  /// </summary>
  /// <typeparam name="T">The type of the items in the result set.</typeparam>
  public PagedResultDto()
  {
  }

  /// <summary>
  /// Represents a paged result containing a collection of items and the total count of all items that match the query criteria.
  /// </summary>
  /// <typeparam name="T">The type of the items in the result.</typeparam>
  public PagedResultDto(IReadOnlyList<T> items, int totalCount) : base(items)
  {
    TotalCount = totalCount;
  }
}
