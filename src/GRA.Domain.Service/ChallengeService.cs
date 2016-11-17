using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseService<ChallengeService>
    {
        public ChallengeService(ILogger<ChallengeService> logger) : base(logger)
        {


        }
    }
}
