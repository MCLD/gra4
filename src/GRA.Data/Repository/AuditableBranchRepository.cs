using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Data.Repository
{
    public class AuditableBranchRepository
        : Abstract.BaseRepository<AuditableBranchRepository>, IBranchRepository
    {
        private readonly GenericAuditableRepository<Model.Branch, Domain.Model.Branch> genericRepo;

        public AuditableBranchRepository(Context context,
            ILogger<AuditableBranchRepository> logger,
            AutoMapper.IMapper mapper) : base(context, logger, mapper)
        {
            genericRepo =
                new GenericAuditableRepository<Model.Branch, Domain.Model.Branch>(context, logger, mapper);
        }
        public void Add(int userId, Domain.Model.Branch entity)
        {
            genericRepo.Add(userId, entity);
        }

        public IQueryable<Domain.Model.Branch> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Branch>();
        }

        public Domain.Model.Branch GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Branch> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.Branch entity)
        {
            genericRepo.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Branch entity)
        {
            genericRepo.Update(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }
    }
}
