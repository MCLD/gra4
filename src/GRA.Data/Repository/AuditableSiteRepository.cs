using GRA.Domain.Repository;
using System.Linq;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace GRA.Data.Repository
{
    public class AuditableSiteRepository
        : Abstract.BaseRepository<AuditableSiteRepository>, ISiteRepository
    {
        private readonly GenericAuditableRepository<Model.Site, Domain.Model.Site> genericRepo;

        public AuditableSiteRepository(
            Context context,
            ILogger<AuditableSiteRepository> logger,
            AutoMapper.IMapper mapper)
            : base(context, logger, mapper)
        {
            genericRepo =
                new GenericAuditableRepository<Model.Site, Domain.Model.Site>(context, logger, mapper);
        }

        public void Add(int userId, Domain.Model.Site entity)
        {
            genericRepo.Add(userId, entity);
        }

        public IQueryable<Domain.Model.Site> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Site>();
        }

        public Domain.Model.Site GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Site> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.Site entity)
        {
            genericRepo.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Site entity)
        {
            genericRepo.Update(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }
    }
}
