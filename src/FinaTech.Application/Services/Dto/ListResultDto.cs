namespace FinaTech.Application.Services.Dto;

public class ListResultDto<T>: IListResult<T>
{
    public IReadOnlyList<T> Items
    {
        get { return _items ??= new List<T>(); }
        set => _items = value;
    }
    private IReadOnlyList<T> _items;

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    protected ListResultDto()
    {

    }

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    /// <param name="items">List of items</param>
    protected ListResultDto(IReadOnlyList<T> items)
    {
        Items = items;
    }
}
