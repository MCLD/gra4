using System.Linq;

namespace GRA.Domain.Repository
{
    public interface IRepository<DomainEntity>
    {
        IQueryable<DomainEntity> PageAll(int skip, int take);

        DomainEntity GetById(int id);

        void Add(DomainEntity entity);

        void Update(DomainEntity entity);

        void Remove(DomainEntity entity);

        void Remove(int id);

        void Save();
    }
}
