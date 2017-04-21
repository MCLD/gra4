using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class DynamicAvatarElementRepository
        : AuditingRepository<Model.DynamicAvatarElement, DynamicAvatarElement>,
        IDynamicAvatarElementRepository
    {
        public DynamicAvatarElementRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarElementRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatarElement>> GetUserAvatar(int userId)
        {
            return await _context.UserDynamicAvatars.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.DynamicAvatarElement)
                .ProjectTo<DynamicAvatarElement>()
                .ToListAsync();
        }
        
    }
}

