namespace InventorySys.Application.Common.Models;

public record PaginatedRequest<T>: IRequest<PaginatedList<T>> where T: class
{
    private const int MaxPageSize = 1000;
    private int _pageSize = 10;

    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
