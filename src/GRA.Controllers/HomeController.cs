using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            HttpContext.Items["sitePath"] = sitePath;
            var site = await GetCurrentSite(sitePath);
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if (AuthUser.Identity.IsAuthenticated)
            {
                return View("Dashboard");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { action = "Index" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogBook(Domain.Model.Book book)
        {
            var result = await _activityService.LogActivityAsync(GetActiveUserId(), 1);
            string message = $"<span class=\"fa fa-star\"></span> You earned <strong>{result.PointsEarned} points</strong> and currently have <strong>{result.User.PointsEarned} points</strong>!";
            if (!string.IsNullOrWhiteSpace(book.Title))
            {
                await _activityService.AddBook(GetActiveUserId(), book);
                message += $" The book <strong><em>{book.Title}</em> by {book.Author}</strong> was added to your book list.";
            }
            AlertSuccess = message;
            return RedirectToAction("Index");
        }
    }
}
