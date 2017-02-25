using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class DynamicViewModel
    {
        public string CurrentlyShown { get; set; }
        public Dictionary<int, string> Paths { get; set; }
    }
}
