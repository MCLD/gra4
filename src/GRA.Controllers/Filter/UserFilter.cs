using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.Filter
{
    public class UserFilter : Attribute, IAsyncActionFilter
    {
        private readonly MailService _mailService;
        private readonly UserService _userService;

        public UserFilter(MailService mailService, UserService userService)
        {
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var pendingQuestionnaire = httpContext.Session.GetInt32(SessionKey.PendingQuestionnaire);
                if (pendingQuestionnaire.HasValue)
                {
                    context.ActionDescriptor.RouteValues.TryGetValue("action", out string action);
                    if (action != "Signout")
                    {
                        var controller = (Base.Controller)context.Controller;
                        context.Result = controller.RedirectToAction("Index", "Questionnaires", new { id = pendingQuestionnaire });
                        return;
                    }
                }
                httpContext.Items[ItemKey.UnreadCount] = await _mailService.GetUserUnreadCountAsync();
            }
            await next();
        }
    }
}
