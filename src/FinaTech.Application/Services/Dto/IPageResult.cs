namespace FinaTech.Application.Services.Dto;

/// <summary>
/// Represents a paged result containing a collection of items and additional metadata, inheriting from the base list result.
/// </summary>
/// <typeparam name="T">The type of elements contained in the paged result.</typeparam>
public interface IPageResult<T> : IListResult<T>
{
}
