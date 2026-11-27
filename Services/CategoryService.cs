using Microsoft.EntityFrameworkCore;
using ProductStore.Data;
using ProductStore.Models;

namespace ProductStore.Services;

public class CategoryService(AppDbContext context) : ICategoryService
{
    public Task<List<Category>> GetAllAsync() =>
        context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public Task<Category?> GetByIdAsync(int id) =>
        context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Category?> GetWithProductsAsync(int id) =>
        context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> NameExistsAsync(string name, int? exceptId = null) =>
        context.Categories.AnyAsync(c => c.Name == name && c.Id != exceptId);

    public async Task CreateAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }

    // false when the category still has products
    public async Task<bool> DeleteAsync(int id)
    {
        if (await context.Products.AnyAsync(p => p.CategoryId == id))
            return false;

        var category = await context.Categories.FindAsync(id);
        if (category is null)
            return true;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }
}
