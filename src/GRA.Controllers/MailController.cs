﻿using GRA.Controllers.ViewModel.Mail;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    [Authorize]
    public class MailController : Base.UserController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MailService _mailService;

        public MailController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            MailService mailService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            PageTitle = "My Profile";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetUserPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = new List<MailItemViewModel>(),
                PaginateModel = paginateModel
            };

            foreach (var item in mailList.Data)
            {
                string toFrom;
                bool isNew = false;
                if (item.ToUserId != null)
                {
                    toFrom = "To you";
                    if (item.IsNew == true)
                    {
                        isNew = true;
                    }
                }
                else
                {
                    toFrom = "From you";
                }
                MailItemViewModel mailItem = new MailItemViewModel()
                {
                    Id = item.Id,
                    Date = item.CreatedAt.ToString("d"),
                    ToFrom = toFrom,
                    Subject = item.Subject,
                    IsNew = isNew
                };
                viewModel.Mail.Add(mailItem);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Read(int id)
        {
            var mail = await _mailService.GetDetails(id);
            return View(mail);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}