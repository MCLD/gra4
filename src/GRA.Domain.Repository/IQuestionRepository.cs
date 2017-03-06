using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<ICollection<Question>> GetByQuestionnaireIdAsync(int questionnaireId);
    }
}
