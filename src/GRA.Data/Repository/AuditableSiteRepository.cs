using GRA.Domain.Repository;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AuditableSiteRepository : BaseRepository<AuditableSiteRepository>, ISiteRepository
    {
        private readonly GenericAuditableRepository<Model.Site, Domain.Model.Site> genericAuditableRepository;

        public AuditableSiteRepository(
            Context context, 
            ILogger<AuditableSiteRepository> logger, 
            AutoMapper.IMapper mapper)
            : base(context, logger, mapper)
        {
            genericAuditableRepository = 
                new GenericAuditableRepository<Model.Site, Domain.Model.Site>(context, logger, mapper);
        }

        public void Add(int userId, Domain.Model.Site entity)
        {
            genericAuditableRepository.Add(userId, entity);
        }

        public IQueryable<Domain.Model.Site> GetAll()
        {
            return genericAuditableRepository.GetAll();
        }

        public Domain.Model.Site GetById(int id)
        {
            return genericAuditableRepository.GetById(id);
        }

        public IQueryable<Domain.Model.Site> PageAll(int skip, int take)
        {
            return genericAuditableRepository.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericAuditableRepository.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.Site entity)
        {
            genericAuditableRepository.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Site entity)
        {
            genericAuditableRepository.Update(userId, entity);
        }
    }
}
