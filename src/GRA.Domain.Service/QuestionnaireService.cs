using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
                return addedQuestionnaire;
            }
        }

        public async Task<Questionnaire> UpdateAsync(Questionnaire questionnaire)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            var currentQuestionnaire = await _questionnaireRepository.GetByIdAsync(questionnaire.Id);
            if (currentQuestionnaire.IsLocked)
            {
                _logger.LogError($"User {authId} cannot update locked questionnaire {currentQuestionnaire.Id}.");
                throw new GraException("Questionnaire is locked and cannot be edited.");
            }

            currentQuestionnaire.Name = questionnaire.Name;
            currentQuestionnaire.IsLocked = questionnaire.IsLocked;
            return await _questionnaireRepository.UpdateSaveAsync(authId, currentQuestionnaire);
        }

        public async Task UpdateQuestionListAsync(int questionnaireId, List<int> questionOrderList)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            var questions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);
            var questionsIdList = questions.Select(_ => _.Id);
            var invalidQuestions = questionOrderList.Except(questionsIdList);
            if (invalidQuestions.Any())
            {
                _logger.LogError($"User {authId} cannot update question {invalidQuestions.First()} for questionnaire {questionnaireId}.");
                throw new GraException("Invalid question selection.");
            }

            var questionUpdateList = questions.Where(_ => questionOrderList.Contains(_.Id));
            foreach (var question in questionUpdateList)
            {
                question.SortOrder = questionOrderList.IndexOf(question.Id);
                await _questionRepository.UpdateSaveAsync(authId, question);
            }

            var questionDeleteList = questions.Except(questionUpdateList);
            foreach (var question in questionDeleteList)
            {
                await _questionRepository.RemoveSaveAsync(authId, question.Id);
            }
        }

        // add question and answers to questionnaire
        public async Task<Questionnaire> AddQuestionsAsync(int questionnaireId,
            IEnumerable<Question> questions)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            foreach (var question in questions)
            {
                question.QuestionnaireId = questionnaireId;
                var addedQuestion = await _questionRepository.AddSaveAsync(authId, question);
                foreach (var answer in question.Answers)
                {
                    answer.QuestionId = addedQuestion.Id;
                    await _answerRepository.AddAsync(authId, answer);
                }
                await _answerRepository.SaveAsync();
            }

            return await _questionnaireRepository.GetByIdAsync(questionnaireId);
        }

        public async Task RemoveAsync(int id)
        {
            VerifyManagementPermission();
            await _questionnaireRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), id);
        }

        public async Task<Questionnaire> GetByIdAsync(int questionnaireId, bool includeAnswers)
        {
            var questionnaire = await _questionnaireRepository
                .GetByIdAsync(questionnaireId, includeAnswers);

            if (questionnaire == null)
            {
                throw new GraException("The requested questionnaire could not be accessed or does not exist.");
            }

            return questionnaire;
        }

        public async Task<ICollection<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _answerRepository.GetByQuestionIdAsync(questionId);
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            return await _questionRepository.GetByIdAsync(questionId);
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
            VerifyManagementPermission();
            return await _questionRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), question);
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            return await _questionRepository.UpdateSaveAsync(authId, question);
        }

        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            VerifyManagementPermission();
            return await _answerRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), answer);
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            VerifyManagementPermission();
            await _answerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), answer);
        }

        public async Task RemoveAnswerAsync(int answerId)
        {
            VerifyManagementPermission();
            await _answerRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), answerId);
        }
    }
}
