using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductStore.Models;

namespace ProductStore.ViewModels;

public class ProductFormViewModel
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, 100_000)]
    public int Stock { get; set; }

    [Display(Name = "Category")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a category")]
    public int CategoryId { get; set; }

    // only for the dropdown: it is not posted and not validated
    [BindNever, ValidateNever]
    public IEnumerable<SelectListItem> Categories { get; set; } = [];

    // the file picked in the form; optional
    public IFormFile? Image { get; set; }

    // the image the product already has (only used to show it in Edit)
    [BindNever, ValidateNever]
    public string? ImagePath { get; set; }

    public static ProductFormViewModel From(Product p) => new()
    {
        Name = p.Name, Price = p.Price,
        Stock = p.Stock, CategoryId = p.CategoryId,
        ImagePath = p.ImagePath
    };

    public Product ToEntity() => new()
    {
        Name = Name, Price = Price,
        Stock = Stock, CategoryId = CategoryId
    };

    public void CopyTo(Product p)
    {
        p.Name = Name;
        p.Price = Price;
        p.Stock = Stock;
        p.CategoryId = CategoryId;
    }
}
