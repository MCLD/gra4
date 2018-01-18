﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class GroupTypeRepository : AuditingRepository<Model.GroupType, Domain.Model.GroupType>,
        IGroupTypeRepository
    {
        public GroupTypeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<GroupTypeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<(IEnumerable<GroupType>, int)> GetAllPagedAsync(int siteId,
            int skip,
            int take)
        {
            var count = await DbSet.CountAsync();
            var list = await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .Skip(skip)
                .Take(take)
                .ProjectTo<GroupType>()
                .ToListAsync();
            return (list, count);
        }
    }
}
