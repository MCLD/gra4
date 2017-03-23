using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class RequiredQuestionnaireRepository 
        : AuditingRepository<Model.RequiredQuestionnaire, RequiredQuestionnaire>, IRequiredQuestionnaireRepository
    {
        public RequiredQuestionnaireRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RequiredQuestionnaireRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int?> GetForUser(int siteId, int userId, int? userAge)
        {
            var time = DateTime.Now;
            var questionnaireId = await DbSet.AsNoTracking()
                                    .Where(_ => _.SiteId == siteId
                                        && (_.AgeMinimum.HasValue == false || _.AgeMinimum <= userAge)
                                        && (_.AgeMaximum.HasValue == false || _.AgeMaximum >= userAge)
                                        && (_.StartDate.HasValue == false || _.StartDate <= time)
                                        && (_.EndDate.HasValue == false || _.EndDate >= time))
                                    .Select(_ => _.QuestionnaireId)
                                    .Except(_context.UserQuestionnaires
                                                .Where(_ => _.UserId == userId)
                                                .Select(_ => _.QuestionnaireId))
                                    .FirstOrDefaultAsync();
            if (questionnaireId > 0)
            {
                return questionnaireId;
            }
            else
            {
                return null;
            }
        }
    }
}
