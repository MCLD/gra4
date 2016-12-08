using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.ServiceFacade;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class BadgeRepository 
        : AuditingRepository<Model.Badge, Domain.Model.Badge>, IBadgeRepository
    {
        public BadgeRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<BadgeRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
