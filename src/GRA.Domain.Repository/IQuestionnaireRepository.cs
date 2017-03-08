using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IQuestionnaireRepository : IRepository<Questionnaire>
    {
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<Questionnaire>> PageAsync(BaseFilter filter);
        Task<Questionnaire> GetFullQuestionnaireAsync(int id);
    }
}
