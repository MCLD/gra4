using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class DynamicAvatarModel
    {
        public ICollection<DynamicAvatarLayer> Layers { get; set; }

        public class DynamicAvatarLayer
        {
            public int Id { get; set; }

            public ICollection<int> Items { get; set; }
            public ICollection<string> Colors { get; set; }
        }
    }
}
