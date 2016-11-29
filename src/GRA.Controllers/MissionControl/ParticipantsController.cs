using GRA.Controllers.ViewModel.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.Controller
    {
        private readonly ILogger<ParticipantsController> _logger;
        private readonly MailService _mailService;
        private readonly UserService _userService;
        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            MailService mailService,
            UserService userService)
            : base(context)
        {
            this._logger = Require.IsNotNull(logger, nameof(logger));
            this._mailService = Require.IsNotNull(mailService, nameof(mailService));
            this._userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Participants";
        }

        #region Index
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var participantsList = await _userService
                .GetPaginatedUserListAsync(CurrentUser, skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = participantsList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            ParticipantsListViewModel viewModel = new ParticipantsListViewModel()
            {
                Users = participantsList.Data,
                PaginateModel = paginateModel,
                Search = search,
                CanRemoveParticipant = UserHasPermission(Permission.DeleteParticipants),
                CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.DeleteParticipants)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Remove(CurrentUser, id);
            return RedirectToAction("Index");
        }
        #endregion

        #region Detail
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userService.GetDetails(CurrentUser, id);
            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                User = user,
                Id = user.Id,
                CanEditDetails = UserHasPermission(Permission.EditParticipants)
            };
            PageTitle = $"Participant - {user.Username}";
            return View(viewModel);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> Detail(ParticipantsDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.Update(CurrentUser, model.User);
                AlertSuccess = "Participant infomation was updated";
                return RedirectToAction("Detail", new { id = model.User.Id });
            }
            else
            {
                PageTitle = $"Participant - {model.User.Username}";
                return View(model);
            }
        }
        #endregion

        #region Family
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Family(int id)
        {
            PageTitle = "Participant Family";
            await _userService.GetDetails(User, id);
            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                Id = id
            };
            return View("Family", viewModel);
        }
        #endregion

        #region Books
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Books(int id)
        {
            PageTitle = "Books Read";
            await _userService.GetDetails(User, id);
            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                Id = id
            };
            return View(viewModel);
        }
        #endregion

        #region Points
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Points(int id)
        {
            var user = await _userService.GetDetails(CurrentUser, id);
            PageTitle = $"Participant - {user.Username}";

            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                Id = id
            };
            return View(viewModel);
        }
        #endregion

        #region Mail
        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            var user = await _userService.GetDetails(CurrentUser, id);
            PageTitle = $"Participant - {user.Username}";

            int take = 15;
            int skip = take * (page - 1);

            var mail = await _mailService.GetUserPaginatedAsync(CurrentUser, id, skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mail.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mail.Data,
                PaginateModel = paginateModel,
                Id = id,
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };
            return View(viewModel);
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> MailDetail(int id)
        {
            var mail = await _mailService.GetDetails(CurrentUser, id);
            var userId = mail.ToUserId ?? mail.FromUserId;

            var user = await _userService.GetDetails(CurrentUser, userId);

            PageTitle = $"{(mail.ToUserId.HasValue ? "To" : "From")}: {user.Username}";

            MailDetailViewModel viewModel = new MailDetailViewModel
            {
                Mail = mail,
                Id = userId,
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.DeleteAnyMail)]
        public async Task<IActionResult> DeleteMail(int id)
        {
            var mail = await _mailService.GetDetails(CurrentUser, id);
            var userId = mail.ToUserId ?? mail.FromUserId;

            await _mailService.RemoveAsync(CurrentUser, id);

            return RedirectToAction("Mail", new { id = userId });
        }
        #endregion

        #region PasswordReset
        #endregion
    }
}
