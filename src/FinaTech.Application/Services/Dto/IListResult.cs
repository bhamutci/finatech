namespace FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a result that contains a collection of items.
/// </summary>
/// <typeparam name="T">The type of elements contained in the list.</typeparam>
public interface IListResult<T>
{
    IReadOnlyList<T> Items { get; set; }
}
