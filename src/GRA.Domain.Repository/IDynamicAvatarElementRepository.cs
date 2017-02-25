using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarElementRepository : IRepository<Model.DynamicAvatarElement>
    {
        Task<bool> ExistsAsync(int dynamicAvatarLayerId, int id);
        Task<int> GetIdByLayerIdAsync(int dynamicAvatarLayerId);
    }
}
