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
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class ParticipantsController : Base.Controller
    {
        private readonly ILogger<ParticipantsController> logger;
        private readonly UserService userService;
        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            UserService userService)
            : base(context)
        {
            this.logger = Require.IsNotNull(logger, nameof(logger));
            this.userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Participants";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            IEnumerable<GRA.Domain.Model.User> participantsList = Enumerable.Empty<GRA.Domain.Model.User>();

            await userService.AuthenticateUserAsync(search, null);

            ParticipantsListViewModel viewModel = new ParticipantsListViewModel()
            {
                Users = participantsList,
                Search = search,
                CanRemoveParticipant = false
            };

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = participantsList.Count(),
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
            viewModel.PaginateModel = paginateModel;

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            await userService.AuthenticateUserAsync("blah", null);

            return View();
        }
    }
}
