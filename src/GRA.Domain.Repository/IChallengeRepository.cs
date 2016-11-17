using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IAuditableRepository<Model.Challenge>
    {
        void AddChallengeTaskType(int userId, string name);
    }
}
