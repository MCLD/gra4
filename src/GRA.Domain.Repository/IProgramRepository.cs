using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IProgramRepository : IAuditableRepository<Model.Program>
    {
        IQueryable<Model.Program> GetAll();
    }
}
