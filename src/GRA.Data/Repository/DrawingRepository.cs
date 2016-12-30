using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DrawingRepository
        : AuditingRepository<Model.Drawing, Domain.Model.Drawing>, IDrawingRepository
    {
        public DrawingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.DrawingCriterion)
                    .Where(_ => _.DrawingCriterion.SiteId == siteId)
                    .OrderByDescending(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<Drawing>()
                    .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.DrawingCriterion)
                .Where(_ => _.DrawingCriterion.SiteId == siteId)
                .CountAsync();
        }
    }
}
