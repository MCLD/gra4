using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class QuestionRepository
        : AuditingRepository<Model.Question, Question>, IQuestionRepository
    {
        public QuestionRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<QuestionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Question>> GetByQuestionnaireIdAsync(int questionnaireId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.QuestionnaireId == questionnaireId && _.IsDeleted == false)
                .ProjectTo<Question>()
                .ToListAsync();
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
