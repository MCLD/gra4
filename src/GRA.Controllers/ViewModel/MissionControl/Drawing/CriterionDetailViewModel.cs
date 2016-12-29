using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class CriterionDetailViewModel
    {
        public GRA.Domain.Model.DrawingCriterion Criterion { get; set; }
        public SelectList BranchList { get; set; }
        public bool ReadABook { get; set; }
    }
}
