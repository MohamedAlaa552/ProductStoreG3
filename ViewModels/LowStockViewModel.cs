using ProductStore.Models;

namespace ProductStore.ViewModels;

public class LowStockViewModel
{
    public int Threshold { get; init; }
    public List<Product> Items { get; init; } = [];
}
