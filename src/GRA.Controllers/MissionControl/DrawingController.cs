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
            PageTitle = "Criteria";

            var brancList = await _siteService.GetBranches(1);
            CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
            {
                BranchList = new SelectList(brancList.ToList(), "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriteriaCreate(CriterionDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReadABook)
                {
                    model.Criterion.PointTranslationId = 1;
                    model.Criterion.ActivityAmount = 1;
                }
                var criterion = await _drawingService.AddCriterionAsync(model.Criterion);
                AlertSuccess = ($"Criteria <strong>{criterion.Name}</strong> created");
                return RedirectToAction("CriteriaDetail", new { id = criterion.Id });
            }
            else
            {
                PageTitle = "Criteria";
                var brancList = await _siteService.GetBranches(1);
                model.BranchList = new SelectList(brancList.ToList(), "Id", "Name");
                return View(model);
            }
        }

        public async Task<IActionResult> CriteriaDetail(int id)
        {
            PageTitle = "Criteria";
            var criterion = await _drawingService.GetCriterionDetails(id);
            var brancList = await _siteService.GetBranches(1);
            CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
            {
                Criterion = criterion,
                BranchList = new SelectList(brancList.ToList(), "Id", "Name"),
                ReadABook = criterion.ActivityAmount.HasValue
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriteriaDetail(CriterionDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReadABook)
                {
                    model.Criterion.PointTranslationId = 1;
                    model.Criterion.ActivityAmount = 1;
                }
                else
                {
                    model.Criterion.PointTranslationId = null;
                    model.Criterion.ActivityAmount = null;
                }
                var criterion = await _drawingService.EditCriterionAsync(model.Criterion);
                AlertSuccess = ($"Criteria <strong>{criterion.Name}</strong> saved");
                return RedirectToAction("CriteriaDetail", new { id = criterion.Id });
            }
            else
            {
                PageTitle = "Criteria";
                var brancList = await _siteService.GetBranches(1);
                model.BranchList = new SelectList(brancList.ToList(), "Id", "Name");
                return View(model);
            }
        }
    }
}
