using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class MailRepository
        : AuditingRepository<Model.Mail, Domain.Model.Mail>, IMailRepository
    {
        public MailRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<MailRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
