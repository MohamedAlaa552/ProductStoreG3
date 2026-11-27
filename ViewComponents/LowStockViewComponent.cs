using Microsoft.AspNetCore.Mvc;
using ProductStore.Services;
using ProductStore.ViewModels;

namespace ProductStore.ViewComponents;

public class LowStockViewComponent(IProductService products) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int threshold = 20)
    {
        var items = await products.GetLowStockAsync(threshold);
        return View(new LowStockViewModel { Threshold = threshold, Items = items });
    }
}
