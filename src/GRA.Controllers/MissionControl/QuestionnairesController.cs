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

        public IActionResult Create()
        {
            PageTitle = "Create Questionnaire";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var questionnaire = await _questionnaireServce.AddAsync(model);
                    ShowAlertSuccess($"Questionnaire '{questionnaire.Name}' successfully created!");
                    return RedirectToAction("Edit", new { id = questionnaire.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to create questionnaire: ", gex);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _questionnaireServce.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var questionnaire = await _questionnaireServce.UpdateAsync(model);
                    ShowAlertSuccess($"Questionnaire '{questionnaire.Name}' successfully updated!");
                    return RedirectToAction("Edit", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to create questionnaire: ", gex);
                }
            }
            return View(model);
        }
    }
}
