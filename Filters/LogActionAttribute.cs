using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductStore.Filters;

public class LogActionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.ActionDescriptor.DisplayName;

        Console.WriteLine(action + " started at " + DateTime.Now);
    }
}
