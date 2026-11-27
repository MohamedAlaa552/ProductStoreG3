using System.ComponentModel.DataAnnotations;

namespace ProductStore.ViewModels;

public class CategoryFormViewModel
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = "";
}
