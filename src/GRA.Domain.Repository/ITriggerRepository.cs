using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        Task<ICollection<Trigger>> PageAsync(Filter filter);
        Task<int> CountAsync(Filter filter);
        Task<ICollection<Trigger>> GetTriggersAsync(int userId);
        Task AddTriggerActivationAsync(int userId, int triggerId);
        Task<Trigger> GetByCodeAsync(int siteId, string secretCode);
    }
}
