using GRA.Domain.Repository;
using System;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class SiteRepository : ISiteRepository
    {
        private readonly Context context;
        private readonly AutoMapper.IMapper mapper;
        public SiteRepository(Context context, AutoMapper.IMapper mapper)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.context = context;
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            this.mapper = mapper;
        }
        public void Add(int userId, Domain.Model.Site entity)
        {
            context.Sites.Add(mapper.Map<Domain.Model.Site, Model.Site>(entity));
        }

        public IQueryable<Domain.Model.Site> GetAll()
        {
            return context.Sites.OrderBy(s => s.Name).ProjectTo<Domain.Model.Site>();
        }

        public Domain.Model.Site GetById(int id)
        {
            var site = context.Sites.Where(_ => _.Id == id).SingleOrDefault();
            return site == null
                ? null
                : mapper.Map<Model.Site, Domain.Model.Site>(site);
        }

        public IQueryable<Domain.Model.Site> PageAll(int skip, int take)
        {
            return context.Sites
                .OrderBy(s => s.Name)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Domain.Model.Site>();
        }

        public void Remove(int userId, int id)
        {
            var site = context.Sites.Where(_ => _.Id == id).SingleOrDefault();
            if (site != null)
            {
                context.Sites.Remove(site);
            }
        }

        public void Remove(int userId, Domain.Model.Site entity)
        {
            Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Site entity)
        {
            var site = context.Sites.Where(_ => _.Id == entity.Id).SingleOrDefault();
            mapper.Map<Domain.Model.Site, Model.Site>(entity);
        }
    }
}
