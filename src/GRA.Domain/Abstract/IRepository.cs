﻿using System.Collections.Generic;

namespace GRA.Domain
{
    public interface IRepository
    {
        IEnumerable<Model.Site> GetSites();
    }
}
