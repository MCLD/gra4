using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DynamicAvatarItemRepository : AuditingRepository<Model.DynamicAvatarItem, DynamicAvatarItem>,
        IDynamicAvatarItemRepository
    {
        public DynamicAvatarItemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarItemRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
