using System.Linq;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IAuditableRepository<Model.Site>
    {
        IQueryable<Model.Site> GetAll();
    }
}
