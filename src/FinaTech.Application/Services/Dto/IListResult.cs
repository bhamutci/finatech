namespace FinaTech.Application.Services.Dto;

public interface IListResult<T>
{
    IReadOnlyList<T> Items { get; set; }
}
