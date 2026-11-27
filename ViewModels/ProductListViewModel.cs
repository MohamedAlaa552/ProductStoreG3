using Microsoft.AspNetCore.Mvc.Rendering;
using ProductStore.Models;

namespace ProductStore.ViewModels;

public class ProductListViewModel
{
    public PagedResult<Product> Result { get; init; } = new();
    public ProductQuery Query { get; init; } = new();
    public IEnumerable<SelectListItem> Categories { get; init; } = [];

    // the filters travel with every pager link, so page 2 keeps the same search
    public Dictionary<string, string> RouteFor(int page)
    {
        var route = new Dictionary<string, string> { ["page"] = page.ToString() };
        if (!string.IsNullOrWhiteSpace(Query.Search))
            route["search"] = Query.Search;
        if (Query.CategoryId is int categoryId)
            route["categoryId"] = categoryId.ToString();
        if (!string.IsNullOrEmpty(Query.Sort))
            route["sort"] = Query.Sort;
        if (Query.PageSize != 5)
            route["pageSize"] = Query.PageSize.ToString();
        return route;
    }
}
