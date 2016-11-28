using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class MailService : Abstract.BaseService<MailService>
    {
        public MailService(ILogger<MailService> logger) : base(logger)
        {
        }
    }
}
