using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.Controllers.ActionFilter
{
    public class CacheFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {

        }
    }

    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Код до выполнения действия
            Console.WriteLine("Before action (async)");

            var resultContext = await next(); // Вызов действия

            // Код после выполнения действия
            Console.WriteLine("After action (async)");
        }
    }
}
