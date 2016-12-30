using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingNewViewModel
    {
        public GRA.Domain.Model.Drawing Drawing { get; set; }
        public SelectList CriterionList { get; set; }
        public IEnumerable<string> Criteria { get; set; }
        public int EligibileCount { get; set; }

        public DrawingNewViewModel()
        {
            Drawing = new GRA.Domain.Model.Drawing();
            Drawing.WinnerCount = 1;
        }
    }
}
