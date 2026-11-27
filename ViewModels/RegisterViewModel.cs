using System.ComponentModel.DataAnnotations;

namespace ProductStore.ViewModels;

public class RegisterViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(6)]
    public string Password { get; set; } = "";

    [Compare("Password", ErrorMessage = "The passwords do not match")]
    public string ConfirmPassword { get; set; } = "";
}
