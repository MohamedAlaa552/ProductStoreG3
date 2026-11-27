using Microsoft.EntityFrameworkCore;
using ProductStore.Data;
using ProductStore.Models;
using ProductStore.ViewModels;

namespace ProductStore.Services;

public class ProductService(AppDbContext context) : IProductService
{
    public Task<List<Product>> GetAllAsync() =>
        context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<PagedResult<Product>> GetPagedAsync(ProductQuery q)
    {
        IQueryable<Product> query = context.Products
            .AsNoTracking()
            .Include(p => p.Category);

        var search = q.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));

        if (q.CategoryId is int categoryId)
            query = query.Where(p => p.CategoryId == categoryId);

        query = q.Sort switch
        {
            "price" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "stock" => query.OrderBy(p => p.Stock),
            _ => query.OrderBy(p => p.Name)
        };

        var pageSize = Math.Clamp(q.PageSize, 1, 50);
        var total = await query.CountAsync();
        var totalPages = Math.Max((int)Math.Ceiling(total / (double)pageSize), 1);
        var page = Math.Clamp(q.Page, 1, totalPages);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public Task<List<Product>> GetLowStockAsync(int threshold) =>
        context.Products
            .AsNoTracking()
            .Where(p => p.Stock < threshold)
            .OrderBy(p => p.Stock)
            .ToListAsync();

    public Task<Product?> GetByIdAsync(int id) =>
        context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<bool> NameExistsAsync(string name, int? exceptId = null) =>
        context.Products.AnyAsync(p => p.Name == name && p.Id != exceptId);

    public async Task CreateAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null)
            return;

        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}
