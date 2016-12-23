﻿using GRA.Controllers.ViewModel.Mail;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            var mailList = await _mailService.GetUserInboxPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var mail = await _mailService.GetParticipantMailAsync(id);
                if (mail.IsNew)
                {
                    await _mailService.MarkAsReadAsync(id);
                    HttpContext.Items[ItemKey.UnreadCount] =
                        (int)HttpContext.Items[ItemKey.UnreadCount] - 1;
                }
                return View(mail);
            }
            catch (GraException)
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Reply(int id)
        {
            try
            {
                var mail = await _mailService.GetParticipantMailAsync(id);
                MailCreateViewModel viewModel = new MailCreateViewModel()
                {
                    Subject = $"Reply to: {mail.Subject}",
                    InReplyToId = mail.Id,
                    InReplyToSubject = mail.Subject,
                };
                return View(viewModel);
            }
            catch (GraException)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reply(MailCreateViewModel model)
        {
            if (model.InReplyToId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Mail mail = new Mail()
                    {
                        Subject = model.Subject,
                        Body = model.Body,
                        InReplyToId = model.InReplyToId
                    };
                    await _mailService.SendReplyAsync(mail);
                    AlertSuccess = $"Reply \"<strong>{mail.Subject}</strong>\" sent";
                    return RedirectToAction("Read", new { id = mail.InReplyToId });
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(MailCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Mail mail = new Mail()
                {
                    Subject = model.Subject,
                    Body = model.Body
                };
                await _mailService.SendAsync(mail);
                AlertSuccess = $"Mail \"<strong>{mail.Subject}</strong>\" sent";
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _mailService.RemoveAsync(id);
            return RedirectToAction("Index");
        }
    }
}
