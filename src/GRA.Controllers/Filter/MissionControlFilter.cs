﻿using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class MissionControlFilter : Attribute, IAsyncResourceFilter
    {
        private readonly MailService _mailService;

        public MissionControlFilter(MailService mailService)
        {
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, 
            ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.HasClaim(GRA.ClaimType.Permission,
                GRA.Domain.Model.Permission.ReadAllMail.ToString()))
            {
                httpContext.Items[ItemKey.UnreadCount] = await _mailService.GetAdminUnreadCountAsync();
            }

                await next();
        }
    }
}
