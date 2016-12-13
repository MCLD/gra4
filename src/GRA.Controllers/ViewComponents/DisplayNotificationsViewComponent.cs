using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewComponents
{
    [ViewComponent(Name = "DisplayNotifications")]
    public class DisplayNotificationsViewComponent : ViewComponent
    {
        private const int MaxNotifications = 3;

        private readonly UserService _userService;
        public DisplayNotificationsViewComponent(UserService userService)
        {
            _userService = Require.IsNotNull(userService, nameof(userService));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notifications = await _userService.GetNotificationsForUser();

            var notificationList = new List<GRA.Domain.Model.Notification>();
            int? totalPointsEarned = 0;

            foreach (var notification in notifications.Where(m => m.BadgeId != null)
                .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationList.Count < MaxNotifications)
                {
                    notificationList.Add(notification);
                }
            }

            if (notificationList.Count < MaxNotifications)
            {
                foreach (var notification in notifications.Where(m => m.BadgeId == null)
                    .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
                {
                    totalPointsEarned += notification.PointsEarned;
                    if (notificationList.Count < MaxNotifications)
                    {
                        notificationList.Add(notification);
                    }
                }
            }

            DisplayNotificationsViewModel viewModel = new DisplayNotificationsViewModel()
            {
                Notifications = notificationList,
                TotalPointsEarned = totalPointsEarned ?? 0,
                TruncatedList = (notifications.Count() > MaxNotifications ? true : false)
            };

            //HttpContext.Items[ItemKey.NotificationsDisplayed] = true;
            return View("Alert", viewModel);
        }
    }
}
