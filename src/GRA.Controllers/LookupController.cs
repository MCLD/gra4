using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class LookupController : Base.Controller
    {
        private readonly ILogger<LookupController> _logger;
        private readonly SiteService _siteService;

        public LookupController(ILogger<LookupController> logger,
             ServiceFacade.Controller context,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
        }

        public async Task<JsonResult> GetBranches(int? systemId,
            int? branchId,
            bool listAll = false,
            bool prioritize = false)
        {
            var branchList = new List<Branch>();

            if (systemId.HasValue)
            {
                branchList = (await _siteService.GetBranches(systemId.Value)).OrderBy(_ => _.Name)
                    .ToList();
            }
            else if (listAll)
            {
                branchList = (await _siteService.GetAllBranches()).OrderBy(_ => _.Name)
                    .ToList();
            }

            if (prioritize)
            {
                branchList = branchList.OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ToList();
            }

            return Json(new SelectList(branchList, "Id", "Name", branchId));
        }
    }
}
