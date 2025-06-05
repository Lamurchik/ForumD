using Forum.Model.DB;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.Controllers.ActionFilter
{
    public class CacheFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {
            // Код до выполнения действия

            Console.WriteLine("filter");
            var resultContext = await next(); // Вызов действия

            var res = context.Result;
            if(res is IQueryable<Post> qer)
            {
                Console.WriteLine("мы зашли в фильтр");
                Console.WriteLine(qer.ToList());
            }
            
        }
    }   
}
