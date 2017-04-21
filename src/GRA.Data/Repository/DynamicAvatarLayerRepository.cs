using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Collections.ObjectModel;

namespace GRA.Data.Repository
{
    public class DynamicAvatarLayerRepository
        : AuditingRepository<Model.DynamicAvatarLayer, DynamicAvatarLayer>,
        IDynamicAvatarLayerRepository
    {
        public DynamicAvatarLayerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarLayerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatarLayer>> GetRenameThisAsync(int siteId, int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Position)
                .ProjectTo<DynamicAvatarLayer>()
                .ToListAsync();

        }

    }
}
