using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class UserActivityLogger : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var memberId = resultContext.HttpContext.User.GetMemberId();
        var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        await dbContext.Members
            .Where(member => member.Id == memberId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(prop => prop.LastActive, DateTime.UtcNow));
    }
}