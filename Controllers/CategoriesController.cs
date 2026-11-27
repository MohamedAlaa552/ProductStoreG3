using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductStore.Models;
using ProductStore.Services;
using ProductStore.ViewModels;

namespace ProductStore.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController(ICategoryService categories) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await categories.GetAllAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await categories.GetWithProductsAsync(id);
        return category is null ? NotFound() : View(category);
    }

    public IActionResult Create()
    {
        return View(new CategoryFormViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel vm)
    {
        if (await categories.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name), "This category already exists");

        if (!ModelState.IsValid)
            return View(vm);

        await categories.CreateAsync(new Category { Name = vm.Name });
        TempData["Success"] = $"{vm.Name} was created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await categories.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return View(new CategoryFormViewModel { Name = category.Name });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel vm)
    {
        if (await categories.NameExistsAsync(vm.Name, id))
            ModelState.AddModelError(nameof(vm.Name), "This category already exists");
        if (!ModelState.IsValid)
            return View(vm);

        var category = await categories.GetByIdAsync(id);
        if (category is null) return NotFound();

        category.Name = vm.Name;
        await categories.UpdateAsync(category);
        TempData["Success"] = "Category updated";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await categories.GetByIdAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await categories.DeleteAsync(id))
            TempData["Success"] = "Category deleted";
        else
            TempData["Error"] = "This category still has products, so it cannot be deleted";

        return RedirectToAction(nameof(Index));
    }
}
