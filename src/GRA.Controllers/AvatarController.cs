using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Repository.Extensions;
using System;
using System.Text;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        private readonly ILogger<AvatarController> _logger;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly StaticAvatarService _staticAvatarService;
        private readonly UserService _userService;

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            DynamicAvatarService dynamicAvatarService,
            StaticAvatarService staticAvatarService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _staticAvatarService = Require.IsNotNull(staticAvatarService,
                nameof(staticAvatarService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Avatar";
        }

        public async Task<IActionResult> Index(string id)
        {
            var currentSite = await GetCurrentSiteAsync();
            if (currentSite.UseDynamicAvatars)
            {
                return await DynamicIndex(id);
            }
            else
            {
                int? numericId = id == null ? null : (int?)Convert.ToInt32(id);
                return await StaticIndex(numericId);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(AvatarSelectionViewModel model)
        {
            try
            {
                var avatar = await _staticAvatarService.GetByIdAsync(model.Avatar.Id);
                var user = await _userService.GetDetails(GetActiveUserId());
                user.AvatarId = avatar.Id;
                await _userService.Update(user);
                AlertSuccess = "Your avatar has been updated";
                return RedirectToAction("Index", "Home");
            }
            catch (GraException gex)
            {
                AlertInfo = gex.Message;
                return RedirectToAction("Index");
            }
        }

        private async Task<IActionResult> StaticIndex(int? id)
        {
            var avatarList = (await _staticAvatarService.GetAvartarListAsync()).ToList();

            if (avatarList.Count() > 0)
            {
                if (id.HasValue)
                {
                    if (!avatarList.Any(_ => _.Id == id.Value))
                    {
                        id = null;
                    }
                }

                var user = await _userService.GetDetails(GetActiveUserId());
                var viewingAvatarId = id ?? user.AvatarId ?? avatarList.FirstOrDefault().Id;
                var avatar = avatarList.FirstOrDefault(_ => _.Id == viewingAvatarId);
                avatar.Filename = _pathResolver.ResolveContentPath(avatar.Filename);

                var currentIndex = avatarList.FindIndex(_ => _.Id == viewingAvatarId);
                int previousAvatarId;
                int nextAvatarId;
                if (currentIndex == 0)
                {
                    previousAvatarId = avatarList.Last().Id;
                }
                else
                {
                    previousAvatarId = avatarList[currentIndex - 1].Id;
                }

                if (currentIndex == avatarList.Count - 1)
                {
                    nextAvatarId = avatarList.First().Id;
                }
                else
                {
                    nextAvatarId = avatarList[currentIndex + 1].Id;
                }

                AvatarSelectionViewModel viewModel = new AvatarSelectionViewModel()
                {
                    Avatar = avatarList.FirstOrDefault(_ => _.Id == viewingAvatarId),
                    PreviousAvatarId = previousAvatarId,
                    NextAvatarId = nextAvatarId
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> DynamicIndex(string id = default(string))
        {
            if (!string.IsNullOrEmpty(id) && id.Length % 2 != 0)
            {
                return RedirectToAction("Index");
            }

            Dictionary<int, int> avatarLayerElement = null;
            if (!string.IsNullOrEmpty(id))
            {
                var elementIds = new List<int>();
                foreach (string hexString in id.SplitInParts(2))
                {
                    try
                    {
                        elementIds.Add(Convert.ToInt32(hexString, 16));
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Index");
                    }
                }
                avatarLayerElement = await _dynamicAvatarService.ReturnValidated(elementIds);
                if (avatarLayerElement == null)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Avatar",
                        action = "Index",
                        id = string.Empty
                    });
                }
            }
            else
            {
                avatarLayerElement = await _dynamicAvatarService.GetDefaultAvatarAsync();
            }

            var viewModel = new DynamicViewModel();
            viewModel.Paths = new Dictionary<int, string>();

            int siteId = GetCurrentSiteId();
            int zIndex = 1;
            var currentlyShown = new StringBuilder();
            foreach (int layerId in avatarLayerElement.Keys)
            {
                string path = System.IO.Path.Combine($"site{siteId}",
                    "dynamicavatars",
                    $"layer{layerId}",
                    $"{avatarLayerElement[layerId]}.png");
                viewModel.Paths.Add(zIndex, _pathResolver.ResolveContentPath(path));
                currentlyShown.Append(avatarLayerElement[layerId].ToString("x2"));
                zIndex++;
            }
            viewModel.CurrentlyShown = currentlyShown.ToString();
            return View("DynamicIndex", viewModel);
        }

        public async Task<IActionResult> Increase(int id, DynamicViewModel viewModel)
        {
            return await IncreaseOrDecrease(id, viewModel, true);
        }
        public async Task<IActionResult> Decrease(int id, DynamicViewModel viewModel)
        {
            return await IncreaseOrDecrease(id, viewModel, false);
        }

        private async Task<IActionResult> IncreaseOrDecrease(int id, DynamicViewModel viewModel, bool increase)
        {
            var newValue = new StringBuilder();
            int counter = 0;
            foreach (string elementIdHex in viewModel.CurrentlyShown.SplitInParts(2))
            {
                counter++;
                if (counter == id)
                {
                    int elementIdInt = Convert.ToInt32(elementIdHex, 16);
                    if (increase)
                    {
                        elementIdInt 
                            = await _dynamicAvatarService.GetNextElement(counter, elementIdInt);
                    }
                    else
                    {
                        elementIdInt
                            = await _dynamicAvatarService.GetPreviousElement(counter, elementIdInt);
                    }
                    newValue.Append(elementIdInt.ToString("x2"));
                }
                else
                {
                    newValue.Append(elementIdHex);
                }
            }
            return RedirectToRoute(new
            {
                controller = "Avatar",
                action = "Index",
                id = newValue.ToString()
            });

        }
    }
}
