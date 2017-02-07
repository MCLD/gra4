﻿using GRA.Controllers.ViewModel.Profile;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    [Authorize]
    public class ProfileController : Base.UserController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public ProfileController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            Abstract.IPasswordValidator passwordValidator,
            AuthenticationService authenticationService,
            SiteService siteService,
            UserService userService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "My Profile";
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userService.GetDetails(GetActiveUserId());

            int householdCount = await _userService
                .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id);
            var branchList = await _siteService.GetBranches(user.SystemId);
            var programList = await _siteService.GetProgramList();
            var systemList = await _siteService.GetSystemList();

            ProfileDetailViewModel viewModel = new ProfileDetailViewModel()
            {
                User = user,
                HouseholdCount = householdCount,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name")

            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileDetailViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The Zip Code field is required.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.Update(model.User);
                    AlertSuccess = "Updated profile";
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update profile: ", gex);
                }
            }
            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch() { Id = -1 });
            }
            var programList = await _siteService.GetProgramList();
            var systemList = await _siteService.GetSystemList();
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.RequirePostalCode = site.RequirePostalCode;

            return View(model);
        }

        public async Task<IActionResult> Household(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            User activeUser = await _userService.GetDetails(GetActiveUserId());

            User headUser = null;
            if (authUser.HouseholdHeadUserId != null)
            {
                headUser = await _userService.GetDetails((int)authUser.HouseholdHeadUserId);
            }

            var household = await _userService
                .GetPaginatedFamilyListAsync(authUser.HouseholdHeadUserId ?? authUser.Id, skip, take);

            // authUser is the head of the family
            bool authUserIsHead =
                authUser.Id == household.Data.FirstOrDefault()?.HouseholdHeadUserId;

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = household.Count,
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

            HouseholdListViewModel viewModel = new HouseholdListViewModel()
            {
                Users = household.Data,
                PaginateModel = paginateModel,
                HouseholdCount = household.Count,
                HasAccount = !string.IsNullOrWhiteSpace(activeUser.Username),
                Head = headUser ?? authUser,
                AuthUserIsHead = authUserIsHead,
                ActiveUser = GetActiveUserId()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AddHouseholdMember()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            var userBase = new User()
            {
                LastName = authUser.LastName,
                PostalCode = authUser.PostalCode,
                Email = authUser.Email,
                PhoneNumber = authUser.PhoneNumber,
                BranchId = authUser.BranchId,
                SystemId = authUser.SystemId
            };

            var systemList = await _siteService.GetSystemList();
            var branchList = await _siteService.GetBranches(authUser.SystemId);
            var programList = await _siteService.GetProgramList();

            HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
            {
                User = userBase,
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name")
            };

            if (programList.Count() == 1)
            {
                viewModel.User.ProgramId = programList.SingleOrDefault().Id;
            }

            return View("HouseholdAdd", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The Zip Code field is required.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.AddHouseholdMemberAsync(authUser.Id, model.User);
                    AlertSuccess = "Added household member";
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add household member: ", gex);
                }
            }
            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch() { Id = -1 });
            }
            var programList = await _siteService.GetProgramList();
            var systemList = await _siteService.GetSystemList();
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.RequirePostalCode = site.RequirePostalCode;

            return View("HouseholdAdd", model);
        }

        public async Task<IActionResult> AddExistingParticipant()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExistingParticipant(HouseholdExistingViewModel model)
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    string addedMembers = await _userService
                        .AddParticipantToHouseholdAsync(model.Username, model.Password);
                    ShowAlertSuccess(addedMembers + " has been added to your household");
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not add participant to household: ", gex.Message);
                }
            }
                return View(model);
        }

        public IActionResult RegisterHouseholdMember()
        {
            return RedirectToAction("Household");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterHouseholdMember(HouseholdRegisterViewModel model)
        {
            var user = await _userService.GetDetails(model.RegisterId);
            var authUser = GetId(ClaimType.UserId);
            if (user.HouseholdHeadUserId != authUser || !string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Household");
            }

            if (model.Validate)
            {
                if (ModelState.IsValid)
                {
                    user.Username = model.Username;
                    try
                    {
                        await _userService.RegisterHouseholdMemberAsync(user, model.Password);
                        AlertSuccess = "Household member registered!";
                        return RedirectToAction("Household");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to register household member:", gex);
                    }
                }
                return View("HouseholdRegisterMember", model);
            }
            else
            {
                ModelState.Clear();
                return View("HouseholdRegisterMember", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginAs(int loginId)
        {
            var user = await _userService.GetDetails(loginId);
            var authUser = GetId(ClaimType.UserId);
            var activeUser = GetActiveUserId();

            if ((user.Id == authUser || user.HouseholdHeadUserId == authUser) && activeUser != loginId)
            {
                HttpContext.Session.SetInt32(SessionKey.ActiveUserId, loginId);
                ShowAlertSuccess($"You are now signed in as {user.FullName}.", "user");
            }
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        public async Task<IActionResult> Books(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var books = await _userService
                .GetPaginatedUserBookListAsync(GetActiveUserId(), skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = books.Count,
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

            User user = await _userService.GetDetails(GetActiveUserId());

            BookListViewModel viewModel = new BookListViewModel()
            {
                Books = books.Data,
                PaginateModel = paginateModel,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> History(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var history = await _userService
                .GetPaginatedUserHistoryAsync(GetActiveUserId(), skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = history.Count,
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

            User user = await _userService.GetDetails(GetActiveUserId());

            HistoryListViewModel viewModel = new HistoryListViewModel()
            {
                Historys = new List<HistoryItemViewModel>(),
                PaginateModel = paginateModel,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                TotalPoints = user.PointsEarned
            };

            foreach (var item in history.Data)
            {
                if (item.ChallengeId != null)
                {
                    var url = Url.Action("Detail", "Challenges", new { id = item.ChallengeId });
                    item.Description = $"<a target='_blank' href='{url}'>{item.Description}</a>";
                }
                HistoryItemViewModel itemModel = new HistoryItemViewModel()
                {
                    CreatedAt = item.CreatedAt.ToString("d"),
                    Description = item.Description,
                    PointsEarned = item.PointsEarned,
                };
                if (!string.IsNullOrWhiteSpace(item.BadgeFilename))
                {
                    itemModel.BadgeFilename = _pathResolver.ResolveContentPath(item.BadgeFilename);
                }
                viewModel.Historys.Add(itemModel);
            }
            return View(viewModel);
        }

        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userService.GetDetails(GetActiveUserId());
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Index");
            }

            ChangePasswordViewModel viewModel = new ChangePasswordViewModel()
            {
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = true
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userService.GetDetails(GetActiveUserId());
                var loginAttempt = await _authenticationService
                    .AuthenticateUserAsync(user.Username, model.OldPassword);
                if (loginAttempt.PasswordIsValid)
                {
                    try
                    {
                        await _authenticationService.ResetPassword(GetActiveUserId(),
                            model.NewPassword);
                        AlertSuccess = "Password changed";
                        return RedirectToAction("ChangePassword");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to change password:", gex);
                    }
                }
                else
                {
                    model.ErrorMessage = "The username and password entered do not match";
                }
            }
            return View(model);
        }
    }
}
