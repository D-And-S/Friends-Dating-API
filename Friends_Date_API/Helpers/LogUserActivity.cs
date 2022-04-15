using Friends_Date_API.Extension;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Friends_Date_API.Helpers
{
    // We use IasynchActionFilter action filter to do something before execute the request
    // and after executing the request
    public class LogUserActivity : IAsyncActionFilter
    {
        // the next paramter says what's gonna happen after an action is executed
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            //check wheather user authenticated or not
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllsync();

        }
    }
}
