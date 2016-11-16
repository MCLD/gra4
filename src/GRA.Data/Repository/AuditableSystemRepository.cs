using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Data.Repository
{
    public class AuditableSystemRepository
        : Abstract.BaseRepository<AuditableSystemRepository>, ISystemRepository
    {
        private readonly GenericAuditableRepository<Model.System, Domain.Model.System> genericRepo;

        public AuditableSystemRepository(Context context,
            ILogger<AuditableSystemRepository> logger,
            AutoMapper.IMapper mapper) : base(context, logger, mapper)
        {
            genericRepo =
                new GenericAuditableRepository<Model.System, Domain.Model.System>(context, logger, mapper);
        }
        public void Add(int userId, Domain.Model.System entity)
        {
            genericRepo.Add(userId, entity);
        }

        public IQueryable<Domain.Model.System> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.System>();
        }

        public Domain.Model.System GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.System> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.System entity)
        {
            genericRepo.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.System entity)
        {
            genericRepo.Update(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }

    }
}
