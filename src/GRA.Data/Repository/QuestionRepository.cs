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
            var questions = DbSet
                .AsNoTracking()
                .Where(_ => _.QuestionnaireId == questionnaireId)
                .ProjectTo<Question>()
                .ToList();

            throw new NotImplementedException();
            //return questions;
        }
    }
}
