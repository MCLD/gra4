using GRA.Domain.Model;
using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.ServiceFacade;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class QuestionnaireRepository
        : AuditingRepository<Model.Questionnaire, Questionnaire>, IQuestionnaireRepository
    {
        public QuestionnaireRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<QuestionnaireRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
