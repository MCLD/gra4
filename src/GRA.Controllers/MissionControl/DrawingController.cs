using GRA.Controllers.ViewModel.MissionControl.Drawing;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.PerformDrawing)]
    public class DrawingController : Base.MCController
    {

        private readonly ILogger<DrawingController> _logger;
        private readonly DrawingService _drawingService;
        private readonly SiteService _siteService;
        public DrawingController(ILogger<DrawingController> logger,
            ServiceFacade.Controller context,
            DrawingService drawingService,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _drawingService = Require.IsNotNull(drawingService, nameof(drawingService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Drawing";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var drawingList = await _drawingService.GetPaginatedDrawingListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = drawingList.Count,
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

            DrawingListViewModel viewModel = new DrawingListViewModel()
            {
                Drawings = drawingList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Criteria(int page = 1)
        {
            PageTitle = "Criteria";

            int take = 15;
            int skip = take * (page - 1);

            var criterionList = await _drawingService.GetPaginatedCriterionListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = criterionList.Count,
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

            CriterionListViewModel viewModel = new CriterionListViewModel()
            {
               Criteria = criterionList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CriteriaCreate()
        {
            var brancList = await _siteService.GetBranches(1);
            CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
            {
                BranchList = new SelectList(brancList.ToList(), "Id", "Name")
            };
            return View(viewModel);
        }
    }
}
