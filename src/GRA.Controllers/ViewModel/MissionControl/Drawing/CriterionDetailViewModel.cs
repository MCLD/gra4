﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class CriterionDetailViewModel
    {
        public GRA.Domain.Model.DrawingCriterion Criterion { get; set; }
        public SelectList BranchList { get; set; }
        [DisplayName("Require participant to have read a book")]
        public bool ReadABook { get; set; }
    }
}
