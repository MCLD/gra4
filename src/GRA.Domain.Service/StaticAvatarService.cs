using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class StaticAvatarService : Abstract.BaseUserService<StaticAvatarService>
    {
        public StaticAvatarService(ILogger<StaticAvatarService> logger, 
            IUserContextProvider userContextProvider) : base(logger, userContextProvider)
        {
        }
    }
}
