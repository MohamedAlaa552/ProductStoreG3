using Microsoft.EntityFrameworkCore;
using ProductStore.Models;

namespace ProductStore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Stationery" },
            new Category { Id = 2, Name = "Bags" },
            new Category { Id = 3, Name = "Home" });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Notebook", Price = 45, Stock = 120, CategoryId = 1 },
            new Product { Id = 2, Name = "Pen", Price = 8, Stock = 500, CategoryId = 1 },
            new Product { Id = 3, Name = "Stapler", Price = 60, Stock = 40, CategoryId = 1 },
            new Product { Id = 4, Name = "Marker Set", Price = 35, Stock = 85, CategoryId = 1 },
            new Product { Id = 5, Name = "Backpack", Price = 650, Stock = 30, CategoryId = 2 },
            new Product { Id = 6, Name = "Laptop Bag", Price = 480, Stock = 18, CategoryId = 2 },
            new Product { Id = 7, Name = "Lunch Bag", Price = 120, Stock = 55, CategoryId = 2 },
            new Product { Id = 8, Name = "Desk Lamp", Price = 320, Stock = 0, CategoryId = 3 },
            new Product { Id = 9, Name = "Water Bottle", Price = 95, Stock = 75, CategoryId = 3 },
            new Product { Id = 10, Name = "Mug", Price = 40, Stock = 200, CategoryId = 3 });
    }
}
