﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain
{
    public interface IGRARepository : IDisposable
    {
        IEnumerable<Site> GetSites();
    }
}
