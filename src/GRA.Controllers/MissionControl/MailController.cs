using GRA.Controllers.ViewModel.MissionControl.Mail;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ReadAllMail)]
    public class MailController : Base.MCController
    {
        private readonly ILogger<ParticipantsController> _logger;
        private readonly MailService _mailService;
        public MailController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            MailService mailService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            PageTitle = "Mail";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllUnreadPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ViewAll(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }
    }
}
