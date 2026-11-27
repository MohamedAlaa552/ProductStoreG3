namespace ProductStore.ViewModels;

public class PagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 5;
    public int TotalCount { get; init; }

    public int TotalPages => Math.Max((int)Math.Ceiling(TotalCount / (double)PageSize), 1);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
