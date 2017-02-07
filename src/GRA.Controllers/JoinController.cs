﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.ViewModel.Join;
using Microsoft.AspNetCore.Mvc.Rendering;
using GRA.Domain.Service;
using GRA.Domain.Model;
using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers
{
    public class JoinController : Base.UserController
    {
        private readonly ILogger<JoinController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly AuthenticationService _authenticationService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            SchoolService schoolService,
            SiteService siteService,
            UserService userService)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Join";
        }

        public async Task<IActionResult> Index()
        {
            var site = await GetCurrentSiteAsync();
            var siteStage = GetSiteStage();
            if (siteStage == SiteStage.BeforeRegistration)
            {
                if (site.RegistrationOpens.HasValue)
                {
                    AlertInfo = $"You can join {site.Name} on {site.RegistrationOpens.Value.ToString("d")}";
                }
                else
                {
                    AlertInfo = $"Registration for {site.Name} has not opened yet";
                }
                return RedirectToAction("Index", "Home");
            }
            else if (siteStage >= SiteStage.ProgramEnded)
            {
                AlertInfo = $"{site.Name} has ended, please join us next time!";
                return RedirectToAction("Index", "Home");
            }

            PageTitle = $"{site.Name} - Join Now!";

            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            var districtList = await _schoolService.GetDistrictsAsync();

            if (site.SinglePageSignUp)
            {
                SinglePageViewModel viewModel = new SinglePageViewModel()
                {
                    RequirePostalCode = site.RequirePostalCode,
                    ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name")
                    
                };

                if (systemList.Count() == 1)
                {
                    var systemId = systemList.SingleOrDefault().Id;
                    var branchList = await _siteService.GetBranches(systemId);
                    if (branchList.Count() == 1)
                    {
                        branchList = branchList.Prepend(new Branch() { Id = -1 });
                    }
                    else
                    {
                        viewModel.BranchId = branchList.SingleOrDefault().Id;
                    }
                    viewModel.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
                    viewModel.SystemId = systemId;
                }

                if (programList.Count() == 1)
                {
                    viewModel.ProgramId = programList.SingleOrDefault().Id;
                }

                return View(viewModel);
            }

            else
            {
                Step1ViewModel viewModel = new Step1ViewModel();
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(SinglePageViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError("PostalCode", "The Zip Code field is required.");
            }

            bool askAge = false;
            bool askSchool = false;
            if (model.ProgramId.HasValue)
            {
                var program = await _siteService.GetProgramByIdAsync(model.ProgramId.Value);
                askAge = program.AskAge;
                askSchool = program.AskSchool;
                if (program.AgeRequired && !model.Age.HasValue)
                {
                    ModelState.AddModelError("Age", "The Age field is required.");
                }
                if (program.SchoolRequired)
                {
                    if (!model.NewEnteredSchool && !model.SchoolId.HasValue)
                    {
                        ModelState.AddModelError("SchoolId", "The School field is required.");
                    }
                    else if (model.NewEnteredSchool
                        && string.IsNullOrWhiteSpace(model.EnteredSchoolName))
                    {
                        ModelState.AddModelError("EnteredSchoolName", "The School Name field is required.");
                    }
                }
                if (model.NewEnteredSchool && !model.SchoolDistrictId.HasValue
                    && ((program.AskSchool && !string.IsNullOrWhiteSpace(model.EnteredSchoolName))
                        || program.SchoolRequired))
                {
                    ModelState.AddModelError("SchoolDistrictId", "The School District field is required.");
                }
            }

            if (ModelState.IsValid)
            {
                model.SiteId = site.Id;

                if (!askAge)
                {
                    model.Age = null;
                }
                if (askSchool)
                {
                    if (model.NewEnteredSchool)
                    {
                        model.SchoolId = null;
                    }
                    else
                    {
                        model.EnteredSchoolName = null;
                    }
                }
                else
                {
                    model.SchoolId = null;
                    model.EnteredSchoolName = null;
                }

                User user = _mapper.Map<User>(model);
                try
                {
                    await _userService.RegisterUserAsync(user, model.Password, 
                        model.SchoolDistrictId);
                    await LoginUserAsync(await _authenticationService
                        .AuthenticateUserAsync(user.Username, model.Password));

                    return RedirectToAction("Index", "Home");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not create your account: ", gex);
                    if (gex.Message.Contains("password"))
                    {
                        ModelState.AddModelError("Password", "Please correct the issues with your password.");
                    }
                }
            }

            PageTitle = $"{site.Name} - Join Now!";

            if (model.SystemId.HasValue)
            {
                var branchList = await _siteService.GetBranches(model.SystemId.Value);
                if (model.BranchId < 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            }
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = askAge;
            model.ShowSchool = askSchool;

            var districtList = await _schoolService.GetDistrictsAsync();
            if (model.SchoolId.HasValue)
            {
                var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.SchoolId.Value);
                var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDisctrictId);
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                    schoolDetails.SchoolDisctrictId);
                model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                    schoolDetails.SchoolTypeId);
                model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
            }
            else
            {
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                if (model.SchoolDistrictId.HasValue)
                {
                    var typeList = await _schoolService.GetTypesAsync(model.SchoolDistrictId);
                    model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                        model.SchoolTypeId);
                    var schoolList = await _schoolService.GetSchoolsAsync(model.SchoolDistrictId,
                        model.SchoolTypeId);
                    model.SchoolList = new SelectList(schoolList.ToList(), "Id", "Name");
                }
            }

            return View(model);
        }
    }
}
