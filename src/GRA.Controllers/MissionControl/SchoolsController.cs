using GRA.Controllers.ViewModel.MissionControl.Schools;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSchools)]
    public class SchoolsController : Base.MCController
    {
        private readonly ILogger<SchoolsController> _logger;
        private readonly SchoolService _schoolService;
        public SchoolsController(ILogger<SchoolsController> logger,
            ServiceFacade.Controller context,
            SchoolService schoolService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            PageTitle = "Schools";
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var schoolList = await _schoolService.GetPaginatedListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = schoolList.Count,
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

            SchoolsListViewModel viewModel = new SchoolsListViewModel()
            {
                Schools = schoolList.Data.ToList(),
                PaginateModel = paginateModel,
                SchoolDistricts = new SelectList (await _schoolService.GetDistrictsAsync(), "Id", "Name"),
                SchoolTypes = new SelectList(await _schoolService.GetTypesAsync(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditSchool(SchoolsListViewModel model, int listId)
        {
            foreach (string key in ModelState.Keys
                .Where(m => !m.StartsWith($"Schools[{listId}].")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                await _schoolService.UpdateSchoolAsync(model.Schools[listId]);
                AlertSuccess = $"'{model.Schools[listId].Name}' updated";
            }
            else
            {
                ShowAlertDanger("Missing required fields");
            }
            return RedirectToAction("Index");
        }
    }
}
