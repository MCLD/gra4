using GRA.Controllers.ViewModel.Mail;
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
            var mailList = await _mailService.GetUserPaginatedAsync(skip, take);

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

        public IActionResult Read()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Read(int id)
        {
            var mail = await _mailService.GetDetails(id);
            if (mail.IsNew)
            {
                await _mailService.MarkAsReadAsync(id);
                HttpContext.Items[ItemKey.UnreadCount] =
                    (int)HttpContext.Items[ItemKey.UnreadCount] - 1;
            }
            return View(mail);
        }

        [HttpPost]
        public IActionResult Reply()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendReply(Mail mail)
        {
            if (ModelState.IsValid)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Reply");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
