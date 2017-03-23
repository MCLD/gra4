using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IRequiredQuestionnaireRepository : IRepository<RequiredQuestionnaire>
    {
        Task<int?> GetForUser(int siteId, int userId, int? userAge);
    }
}
