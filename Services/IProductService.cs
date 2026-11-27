using ProductStore.Models;
using ProductStore.ViewModels;

namespace ProductStore.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<PagedResult<Product>> GetPagedAsync(ProductQuery query);
    Task<List<Product>> GetLowStockAsync(int threshold);
    Task<Product?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId = null);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}
