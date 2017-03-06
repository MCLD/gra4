using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class QuestionnaireService : BaseUserService<QuestionnaireService>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionnaireRepository _questionnaireRepository;
        public QuestionnaireService(ILogger<QuestionnaireService> logger,
            IUserContextProvider userContextProvider,
            IAnswerRepository answerRepository,
            IQuestionRepository questionRepository,
            IQuestionnaireRepository questionnaireRepository) : base(logger, userContextProvider)
        {
            SetManagementPermission(Permission.ManageQuestionnaires);
            _answerRepository = Require.IsNotNull(answerRepository, nameof(answerRepository));
            _questionRepository = Require.IsNotNull(questionRepository,
                nameof(questionRepository));
            _questionnaireRepository = Require.IsNotNull(questionnaireRepository,
                nameof(questionnaireRepository));
        }

        public async Task<DataWithCount<ICollection<Questionnaire>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Questionnaire>>
            {
                Data = await _questionnaireRepository.PageAsync(filter),
                Count = await _questionnaireRepository.CountAsync(filter)
            };
        }

        // add questionnaire
        public async Task<Questionnaire> AddAsync(Questionnaire questionnaire)
        {
            VerifyManagementPermission();

            questionnaire.SiteId = GetCurrentSiteId();
            questionnaire.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            questionnaire.RelatedSystemId = GetClaimId(ClaimType.SystemId);

            var addedQuestionnaire = await _questionnaireRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId),
                questionnaire);

            if (questionnaire.Questions != null && questionnaire.Questions.Count > 0)
            {
                return await AddQuestionsAsync(addedQuestionnaire.Id, questionnaire.Questions);
            }
            else
            {
                return await GetFullQuestionnaireAsync(addedQuestionnaire.Id);
            }
        }

        // add question and answers to questionnaire
        public async Task<Questionnaire> AddQuestionsAsync(int questionnaireId, 
            IEnumerable<Question> questions)
        {
            VerifyManagementPermission();
            int userId = GetClaimId(ClaimType.UserId);

            foreach (var question in questions)
            {
                question.QuestionnaireId = questionnaireId;
                var addedQuestion = await _questionRepository.AddSaveAsync(userId, question);
                foreach (var answer in question.Answers)
                {
                    answer.QuestionId = addedQuestion.Id;
                    await _answerRepository.AddAsync(userId, answer);
                }
                await _answerRepository.SaveAsync();
            }

            return await GetFullQuestionnaireAsync(questionnaireId);
        }

        private async Task<Questionnaire> GetFullQuestionnaireAsync(int questionnaireId)
        {
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);

            var questions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);

            foreach(var question in questions)
            {
                question.Answers = await _answerRepository.GetByQuestionIdAsync(question.Id);
            }
            questionnaire.Questions = questions;
            return questionnaire;
        }
    }
}
