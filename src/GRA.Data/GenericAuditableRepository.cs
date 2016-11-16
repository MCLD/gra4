using AutoMapper.QueryableExtensions;
using GRA.Data.Abstract;
using GRA.Data.Extension;
using GRA.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace GRA.Data
{
    internal class GenericAuditableRepository<DbEntity, DomainEntity>
        where DbEntity : BaseDbEntity
        where DomainEntity : class
    {
        private readonly Context context;
        private readonly ILogger logger;
        private readonly AutoMapper.IMapper mapper;

        private DbSet<DbEntity> dbSet;
        private DbSet<AuditLog> auditSet;

        internal GenericAuditableRepository(Context context,
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

        private void AuditLog(int userId,
            BaseDbEntity newObject,
            BaseDbEntity priorObject = null)
        {
            var audit = new Data.Model.AuditLog
            {
                EntityType = newObject.GetType().ToString(),
                EntityId = newObject.Id,
                UpdatedBy = userId,
                UpdatedAt = DateTime.Now,
                CurrentValue = JsonConvert.SerializeObject(newObject)
            };
            if (priorObject != null)
            {
                audit.PreviousValue = JsonConvert.SerializeObject(priorObject);
            }
            AuditSet.Add(audit);
            try
            {
                if (context.SaveChanges() != 1)
                {
                    logger.LogError($"Error writing audit log for {newObject.GetType()} id {newObject.Id}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(null, ex, $"Error writing audit log for {newObject.GetType()} id {newObject.Id}");
            }
        }

        protected DbSet<AuditLog> AuditSet {
            get {
                return auditSet ?? (auditSet = context.Set<AuditLog>());
            }
        }
        protected DbSet<DbEntity> DbSet {
            get {
                return dbSet ?? (dbSet = context.Set<DbEntity>());
            }
        }

        public IQueryable<DomainEntity> GetAll()
        {
            return DbSet.ProjectTo<DomainEntity>();
        }

        public IQueryable<DomainEntity> PageAll(int skip, int take)
        {
            return DbSet.Skip(skip).Take(take).ProjectTo<DomainEntity>();
        }

        public virtual DomainEntity GetById(int id)
        {
            return mapper.Map<DbEntity, DomainEntity>(DbSet.Find(id));
        }

        public virtual void Add(int userId, DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
            AuditLog(userId, entity);
        }
        public virtual void Update(int userId, DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            var original = DbSet.Find(entity.Id);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
            AuditLog(userId, entity, original);
        }

        public void Remove(int userId, DomainEntity domainEntity)
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
            AuditLog(userId, null, entity);
        }

        public void Remove(int userId, int id)
        {
            var entity = GetById(id);
            if (entity == null) return;

            Remove(userId, entity);
        }
    }
}