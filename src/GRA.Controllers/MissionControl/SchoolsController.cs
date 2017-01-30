﻿using GRA.Controllers.ViewModel.MissionControl.Schools;
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
                SchoolDistricts = new SelectList(await _schoolService.GetDistrictsAsync(), "Id", "Name"),
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

        public async Task<IActionResult> AddSchool()
        {
            PageTitle = "Add Schools";

            SchoolsAddViewModel viewModel = new SchoolsAddViewModel()
            {
                SchoolDistricts = new SelectList(await _schoolService.GetDistrictsAsync(), "Id", "Name"),
                SchoolTypes = new SelectList(await _schoolService.GetTypesAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSchool(SchoolsAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _schoolService.AddSchool(model.School.Name,
                    model.School.SchoolDistrictId,
                    model.School.SchoolTypeId);

                AlertSuccess = $"Added school '{model.School.Name}'";
                return RedirectToAction("Index");
            }
            else
            {
                PageTitle = "Add School";
                model.SchoolDistricts = new SelectList(await _schoolService.GetDistrictsAsync(), "Id", "Name");
                model.SchoolTypes = new SelectList(await _schoolService.GetTypesAsync(), "Id", "Name");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            await _schoolService.RemoveSchool(id);
            AlertSuccess = "School removed";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Districts(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var districtList = await _schoolService.GetPaginatedDistrictListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = districtList.Count,
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

            DistrictListViewModel viewModel = new DistrictListViewModel()
            {
                SchoolDistricts = districtList.Data.ToList(),
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDistrict(DistrictListViewModel model)
        {
            try
            {
                await _schoolService.AddDistrict(model.District.Name);
                ShowAlertSuccess($"Added School District '{model.District.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add School District: ", gex);
            }
            return RedirectToAction("Districts");
        }

        [HttpPost]
        public async Task<IActionResult> EditDistrict(DistrictListViewModel model)
        {
            try
            {
                await _schoolService.UpdateDistrictAsync(model.District);
                ShowAlertSuccess($"School District '{model.District.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit School District: ", gex);
            }
            return RedirectToAction("Districts");
        }

        public async Task<IActionResult> DeleteDistrict(int id)
        {
            try
            {
                await _schoolService.RemoveDistrict(id);
                AlertSuccess = "School District removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete School District: ", gex);
            }
            return RedirectToAction("Districts");
        }

        public async Task<IActionResult> Types(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var typeList = await _schoolService.GetPaginatedTypeListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = typeList.Count,
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

            TypeListViewModel viewModel = new TypeListViewModel()
            {
                SchoolTypes = typeList.Data.ToList(),
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddType(TypeListViewModel model)
        {
            try
            {
                await _schoolService.AddSchoolType(model.Type.Name);
                ShowAlertSuccess($"Added School Type '{model.Type.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add School Type: ", gex);
            }
            return RedirectToAction("Types");
        }

        [HttpPost]
        public async Task<IActionResult> EditType(TypeListViewModel model)
        {
            try
            {
                await _schoolService.UpdateTypeAsync(model.Type);
                ShowAlertSuccess($"School Type '{model.Type.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit School Type: ", gex);
            }
            return RedirectToAction("Types");
        }

        public async Task<IActionResult> DeleteType(int id)
        {
            try
            {
                await _schoolService.RemoveType(id);
                AlertSuccess = "School Type removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete School Type: ", gex);
            }
            return RedirectToAction("Types");
        }
    }
}
