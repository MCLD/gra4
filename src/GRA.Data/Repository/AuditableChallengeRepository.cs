using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GRA.Data.Repository
{
    public class AuditableChallengeRepository
        : Abstract.BaseRepository<AuditableChallengeRepository>, IChallengeRepository
    {
        private readonly GenericAuditableRepository<Model.Challenge, Domain.Model.Challenge> genericRepo;
        private readonly GenericAuditableRepository<Model.ChallengeTask, Domain.Model.ChallengeTask> genTaskRepo;

        public AuditableChallengeRepository(Context context,
            ILogger<AuditableChallengeRepository> logger,
            IMapper mapper) : base(context, logger, mapper)
        {
            genericRepo =
                new GenericAuditableRepository<Model.Challenge, Domain.Model.Challenge>(context, logger, mapper);
            genTaskRepo =
                new GenericAuditableRepository<Model.ChallengeTask, Domain.Model.ChallengeTask>(context, logger, mapper);
        }

        public void Add(int userId, Domain.Model.Challenge entity)
        {
            foreach(var task in entity.GetTasks())
            {
                if(task.ChallengeTaskTypeId == 0)
                {
                    task.ChallengeTaskTypeId = GetChallengeTypeId(task.ChallengeTaskType.ToString());
                }
            }
            var dbEntity = genericRepo.Map(entity);
            foreach(var task in dbEntity.Tasks)
            {
                task.CreatedAt = DateTime.Now;
                task.CreatedBy = userId;
            }
            genericRepo.Add(userId, dbEntity);
        }

        public IQueryable<Domain.Model.Challenge> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Challenge>();
        }

        public Domain.Model.Challenge GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Challenge> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Remove(int userId, Domain.Model.Challenge entity)
        {
            genericRepo.Remove(userId, entity.Id);
        }

        public void Update(int userId, Domain.Model.Challenge entity)
        {
            genericRepo.Update(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }

        private int GetChallengeTypeId(string name)
        {
            return context.ChallengeTaskTypes
                .AsNoTracking()
                .Where(_ => _.Name == name)
                .Select(_ => _.Id)
                .SingleOrDefault();
        }

        public void AddChallengeTaskType(int userId, string name)
        {
            context.ChallengeTaskTypes.Add(new Model.ChallengeTaskType
            {
                Name = name,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}
