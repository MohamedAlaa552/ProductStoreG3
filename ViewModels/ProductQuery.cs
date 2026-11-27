namespace ProductStore.ViewModels;

// everything the list page can ask for; it is bound from the query string
public class ProductQuery
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
