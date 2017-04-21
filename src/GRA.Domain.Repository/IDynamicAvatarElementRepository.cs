using GRA.Domain.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarElementRepository : IRepository<Model.DynamicAvatarElement>
    {
        Task<ICollection<DynamicAvatarElement>> GetUserAvatar(int userId);
    }
}
