using ProductStore.Models;

namespace ProductStore.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetWithProductsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId = null);
    Task CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
}
