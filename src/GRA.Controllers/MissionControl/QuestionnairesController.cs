using GRA.Controllers.ViewModel.MissionControl.Questionnaires;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
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
    [Authorize(Policy = Policy.ManageQuestionnaires)]
    public class QuestionnairesController : Base.MCController
    {
        private readonly ILogger<QuestionnairesController> _logger;
        private readonly QuestionnaireService _questionnaireServce;
        public QuestionnairesController(ILogger<QuestionnairesController> logger,
           ServiceFacade.Controller context,
           QuestionnaireService questionaireService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _questionnaireServce = Require.IsNotNull(questionaireService, 
                nameof(questionaireService));
            PageTitle = "Questionnaires";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            BaseFilter filter = new BaseFilter(page);
            var questionnaireList = await _questionnaireServce.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = questionnaireList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            QuestionnairesListViewModel viewModel = new QuestionnairesListViewModel()
            {
                Questionnaires = questionnaireList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }
    }
}
