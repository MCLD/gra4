using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class DynamicAvatarViewModel
    {
        public ICollection<DynamicAvatarLayer> Layers { get; set; }

        public string ImagePath { get; set; }
        public string Json { get; set; }
    }
}
