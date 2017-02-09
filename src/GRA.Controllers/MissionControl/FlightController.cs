using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class FlightController : Base.MCController
    {
        private readonly ILogger<FlightController> _logger;
        private readonly VendorCodeService _vendorCodeService;

        public FlightController(ILogger<FlightController> logger,
            ServiceFacade.Controller context,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "Flight Director";
        }

        public IActionResult Index()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = "/MissionControl"
                });
            }

            if (!UserHasPermission(Permission.AccessFlightController))
            {
                // not authorized for Mission Control, redirect to authorization code
                return RedirectToRoute(new
                {
                    area = "MissionControl",
                    controller = "Home",
                    action = "AuthorizationCode"
                });
            }
            return View();
        }

        public async Task<IActionResult> CreateVendorCodes()
        {
            var code = await _vendorCodeService.AddTypeAsync(new VendorCodeType
            {
                Description = "Free Book Code",
                MailSubject = "Here's your Free Book Code!",
                Mail = "Congratulations, you've earned a free book! Your free book code is {Code}!",
            });

            var sw = new Stopwatch();
            sw.Start();
            var generatedCount = await _vendorCodeService.GenerateVendorCodesAsync(code.Id, 50);
            sw.Stop();

            AlertSuccess = $"Generated {generatedCount} codes in {sw.Elapsed.TotalSeconds} seconds of type: {code.Description}";

            return View("Index");
        }
    }
}
