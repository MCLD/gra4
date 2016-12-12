using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class NotificationFilter : Attribute, IAsyncResultFilter
    {
        private const int MaxNotifications = 3;

        private readonly UserService _userService;
        public NotificationFilter(UserService userService)
        {
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var notifications = await _userService.GetNotificationsForUser();
                var notificationList = new List<GRA.Domain.Model.Notification>();

                foreach(var notification in notifications.Where(m => m.BadgeId != null)
                    .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
                {
                    notificationList.Add(notification);
                    if (notificationList.Count >= MaxNotifications)
                    {
                        break;
                    }
                }

                if (notificationList.Count < MaxNotifications)
                {
                    foreach(var notification in notifications.Where(m => m.BadgeId == null)
                        .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
                    {
                        notificationList.Add(notification);
                        if (notificationList.Count >= MaxNotifications)
                        {
                            break;
                        }
                    }
                }
                
                httpContext.Items[ItemKey.NotificicationsList] = 
                    notificationList.OrderByDescending(m => m.PointsEarned).ToList();

                await next();

                if (httpContext.Items[ItemKey.NotificationsDisplayed] != null
                    && (bool)httpContext.Items[ItemKey.NotificationsDisplayed] == true)
                {
                    await _userService.ClearNotificationsForUser();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
