using Microsoft.AspNetCore.Mvc;

namespace ProductStore.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error(int code = 500)
    {
        ViewBag.Code = code;

        if (code == 404)
            ViewBag.Message = "We could not find this page.";
        else
            ViewBag.Message = "Something went wrong on our side.";

        Response.StatusCode = code;
        return View();
    }
}
