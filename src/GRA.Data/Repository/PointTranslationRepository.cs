using GRA.Data.ServiceFacade;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;


namespace GRA.Data.Repository
{
    public class PointTranslationRepository
        : AuditingRepository<Model.PointTranslation, Domain.Model.PointTranslation>, IPointTranslationRepository
    {
        public PointTranslationRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PointTranslationRepository> logger) : base(repositoryFacade, logger) { }
    }
}
