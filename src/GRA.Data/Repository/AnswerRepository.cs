﻿using System;
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
    public class AnswerRepository : AuditingRepository<Model.Answer, Answer>, IAnswerRepository
    {
        public AnswerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AnswerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Answer>> GetByQuestionIdAsync(int questionId)
        {
            var answers = await DbSet
                .AsNoTracking()
                .Where(_ => _.QuestionId == questionId)
                .ProjectTo<Answer>()
                .ToListAsync();

            return answers;
        }
    }
}
