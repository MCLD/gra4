using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Data.Repository
{
    public class AuditableProgramRepository
        : Abstract.BaseRepository<AuditableProgramRepository>, IProgramRepository
    {
        private readonly GenericAuditableRepository<Model.Program, Domain.Model.Program> genericRepo;

        public AuditableProgramRepository(
            Context context,
            ILogger<AuditableProgramRepository> logger,
            AutoMapper.IMapper mapper)
            : base(context, logger, mapper)
        {
            genericRepo =
                new GenericAuditableRepository<Model.Program, Domain.Model.Program>(context, logger, mapper);
        }

        public void Add(int userId, Domain.Model.Program entity)
        {
            genericRepo.Add(userId, entity);
        }

        public IQueryable<Domain.Model.Program> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Program>();
        }

        public Domain.Model.Program GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Program> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.Program entity)
        {
            genericRepo.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Program entity)
        {
            genericRepo.Update(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }
    }

}
