using Common.DTOs;
using Common.Enums;
using Microsoft.IdentityModel.Tokens;
using Persistence.IRepositories;

namespace Application.Validator;

public class AnswerProcessValidator : ICustomValidator<PostUserAnswersRequest>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAnswerOptionRepository _answerOptionRepository;
    private readonly List<Notification> _notifications;

    public AnswerProcessValidator(IQuestionRepository questionRepository, ISurveyRepository surveyRepository,
        IAnswerOptionRepository answerOptionRepository)
    {
        _questionRepository = questionRepository;
        _surveyRepository = surveyRepository;
        _notifications = new List<Notification>();
        _answerOptionRepository = answerOptionRepository;
    }

    public IReadOnlyList<Notification> Notifications => _notifications;

    public async Task<bool> ValidateAsync(PostUserAnswersRequest answerDto)
    {
        if (!Enum.TryParse(typeof(Department), answerDto.Department, out _))
        {
            _notifications.Add(new Notification("Department", "Invalid department value"));
        }

        bool surveyExists = await _surveyRepository.Exists(answerDto.SurveyId);

        if (!surveyExists)
        {
            _notifications.Add(new Notification("SurveyId", "Survey cannot be found"));
        }

        foreach (var questionAnswer in answerDto.QuestionAnswers)
        {
            var question = await _questionRepository.GetWithAnswerOptionsAsync(questionAnswer.QuestionId);

            if (question == null)
            {
                _notifications.Add(new Notification("QuestionId", $"Question not found: {questionAnswer.QuestionId}"));
                continue;
            }

            if ((question.AnswerCategoryType == (int)AnswerCategoryType.ONE_SELECT ||
                 question.AnswerCategoryType == (int)AnswerCategoryType.MULTI_SELECT))
            {
                if (!questionAnswer.AnswerOptionIds.Any())
                {
                    _notifications.Add(new Notification("AnswerOptionIds",
                        $"Answer options are required for question: {questionAnswer.QuestionId}"));
                }
                else if (question.AnswerCategoryType == (int)AnswerCategoryType.ONE_SELECT &&
                         questionAnswer.AnswerOptionIds.Count > 1)
                {
                    _notifications.Add(new Notification("AnswerOptionIds",
                        $"Only one answer option is allowed for question: {questionAnswer.QuestionId}"));
                }

                bool areAllOptionIdsValid =
                    await _answerOptionRepository.AreAllOptionIdsValidAsync(questionAnswer.AnswerOptionIds);

                if (!areAllOptionIdsValid)
                {
                    _notifications.Add(new Notification("AnswerOptionIds",
                        $"Ids are invalid: {string.Join(";", questionAnswer.AnswerOptionIds)}"));
                }

                if (!question.AnswerOptions.IsNullOrEmpty())
                {
                    var questionAnswerOptionsIds = question.AnswerOptions.Select(a => a.Id);
                    bool allElementsContained =
                        questionAnswer.AnswerOptionIds.ToList().All(i => questionAnswerOptionsIds.Contains(i));
                    if (!allElementsContained)
                    {
                        _notifications.Add(new Notification("AnswerOptionIds",
                            $"List values do not match with question answer options for question: {questionAnswer.QuestionId}"));
                    }
                }
            }

            if (question.AnswerCategoryType == (int)AnswerCategoryType.FREE_TEXT &&
                string.IsNullOrWhiteSpace(questionAnswer.AnswerText))
            {
                _notifications.Add(new Notification("AnswerText",
                    $"Answer text is required for question: {questionAnswer.QuestionId}"));
            }
        }

        return !_notifications.Any();
    }
}