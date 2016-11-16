using AutoMapper.QueryableExtensions;
using GRA.Data.Abstract;
using GRA.Data.Extension;
using GRA.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GRA.Data
{
    internal class GenericRepository<DbEntity, DomainEntity>
        where DbEntity : BaseDbEntity
        where DomainEntity : class
    {
        private readonly Context context;
        private readonly ILogger logger;
        private readonly AutoMapper.IMapper mapper;

        private DbSet<DbEntity> dbSet;
        private DbSet<AuditLog> auditSet;

        internal GenericRepository(Context context,
            ILogger logger,
           AutoMapper.IMapper mapper)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.context = context;
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            this.mapper = mapper;
        }

        public DbSet<DbEntity> DbSet {
            get {
                return dbSet ?? (dbSet = context.Set<DbEntity>());
            }
        }

        public IQueryable<DomainEntity> PageAll(int skip, int take)
        {
            return DbSet.AsNoTracking().Skip(skip).Take(take).ProjectTo<DomainEntity>();
        }

        public virtual DomainEntity GetById(int id)
        {
            return mapper.Map<DbEntity, DomainEntity>(DbSet.Find(id));
        }

        public virtual void Add(int userId, DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            entity.CreatedBy = userId;
            entity.CreatedAt = DateTime.Now;
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }
        public virtual void Update(DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            var original = DbSet.Find(entity.Id);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public void Remove(DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public void Remove(int userId, int id)
        {
            var entity = GetById(id);
            if (entity == null) return;

            Remove(entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}