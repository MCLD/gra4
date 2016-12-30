using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take);
        Task<int> GetCountAsync(int siteId);
    }
}
