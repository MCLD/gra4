using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class SchoolsAddViewModel
    {
        public GRA.Domain.Model.School School { get; set; }
        public SelectList SchoolDistricts { get; set; }
        public SelectList SchoolTypes { get; set; }
    }
}
