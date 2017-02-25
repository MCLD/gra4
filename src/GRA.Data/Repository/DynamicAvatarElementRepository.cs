using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

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

        public async Task<bool> ExistsAsync(int dynamicAvatarLayerId, int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .AnyAsync();
        }

        public override Task<DynamicAvatarElement> GetByIdAsync(int id)
        {
            throw new System.Exception("Can not look up dynamic avatar elements by id only.");
        }

        public async Task<DynamicAvatarElement> GetByIdLayerAsync(int id, int dynamicAvatarLayerId)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .SingleOrDefaultAsync();
            if (entity == null)
            {
                throw new System.Exception($"{nameof(DynamicAvatarElement)} id {id} with layer id {dynamicAvatarLayerId} could not be found.");
            }
            return _mapper.Map<DynamicAvatarElement>(entity);
        }

        public async Task<int> GetIdByLayerIdAsync(int dynamicAvatarLayerId)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .FirstOrDefaultAsync();
            if(entity == null)
            {
                throw new Exception("Couldn't find an appropriate avatar part!");
            }
            return entity.Id;
        }
    }
}
