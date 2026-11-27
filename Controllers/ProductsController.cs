using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductStore.Filters;
using ProductStore.Services;
using ProductStore.ViewModels;

namespace ProductStore.Controllers;

[LogAction]
[Authorize(Roles = "Admin")]
public class ProductsController(
    IProductService products,
    ICategoryService categories,
    IImageStorage images) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(ProductQuery query)
    {
        var result = await products.GetPagedAsync(query);

        var vm = new ProductListViewModel
        {
            Result = result,
            // show the values that were actually used (page and page size are clamped)
            Query = new ProductQuery
            {
                Search = query.Search,
                CategoryId = query.CategoryId,
                Sort = query.Sort,
                Page = result.Page,
                PageSize = result.PageSize
            },
            Categories = new SelectList(await categories.GetAllAsync(), "Id", "Name", query.CategoryId)
        };
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : View(product);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildFormAsync(new ProductFormViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel vm)
    {
        if (await products.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name),
                "A product with this name already exists");

        if (vm.Image is not null && !images.IsValid(vm.Image, out var imageError))
            ModelState.AddModelError(nameof(vm.Image), imageError);

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(vm));

        var product = vm.ToEntity();
        if (vm.Image is not null)
            product.ImagePath = await images.SaveAsync(vm.Image);

        await products.CreateAsync(product);

        TempData["Success"] = $"{product.Name} was created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return View(await BuildFormAsync(ProductFormViewModel.From(product)));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel vm)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        if (await products.NameExistsAsync(vm.Name, id))
            ModelState.AddModelError(nameof(vm.Name), "A product with this name already exists");
        if (vm.Image is not null && !images.IsValid(vm.Image, out var imageError))
            ModelState.AddModelError(nameof(vm.Image), imageError);

        if (!ModelState.IsValid)
        {
            vm.ImagePath = product.ImagePath;
            return View(await BuildFormAsync(vm));
        }

        vm.CopyTo(product);
        if (vm.Image is not null)
            product.ImagePath = await images.SaveAsync(vm.Image);

        await products.UpdateAsync(product);
        TempData["Success"] = "Product updated";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await products.GetByIdAsync(id);
        await products.DeleteAsync(id);
        images.Delete(product?.ImagePath);

        TempData["Success"] = "Product deleted";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProductFormViewModel> BuildFormAsync(ProductFormViewModel vm)
    {
        vm.Categories = new SelectList(await categories.GetAllAsync(), "Id", "Name", vm.CategoryId);
        return vm;
    }
}
