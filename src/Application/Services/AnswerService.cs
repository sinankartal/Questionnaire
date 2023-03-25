using Application.Validator;
using AutoMapper;
using Common.DTOs;
using Common.Enums;
using Common.Requests;
using Common.Responses;
using Microsoft.IdentityModel.Tokens;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICustomValidator<PostUserAnswersRequest> _processValidatior;
    private readonly ICustomValidator<GetUserSurveyAnswersRequest> _getUserSurveyAnswerValidator;
    private readonly IMapper _mapper;

    public AnswerService(IAnswerRepository answerRepository, ICustomValidator<PostUserAnswersRequest> processValidatior,
        ISurveyRepository surveyRepository,
        ICustomValidator<GetUserSurveyAnswersRequest> getUserSurveyAnswerValidator, IMapper mapper)
    {
        _answerRepository = answerRepository;
        _surveyRepository = surveyRepository;
        _processValidatior = processValidatior;
        _getUserSurveyAnswerValidator = getUserSurveyAnswerValidator;
        _mapper = mapper;
    }

    public async Task<Response> ProcessAsync(PostUserAnswersRequest dto)
    {
        bool isValid = await _processValidatior.ValidateAsync(dto);
        if (!isValid)
        {
            return Response.Failure(_processValidatior.Notifications);
        }

        foreach (var answer in dto.QuestionAnswers)
        {
            if (!answer.AnswerOptionIds.IsNullOrEmpty())
            {
                await SaveAnswerWithOptionsAsync(dto, answer);
            }
            else
            {
                await SaveAnswerWithTextAsync(dto, answer);
            }
        }

        return Response.Success();
    }

    public async Task<TypedResponse<List<AnswerDTO>>> GetUserSurveyAnswers(GetUserSurveyAnswersRequest request)
    {
        bool isValid = await _getUserSurveyAnswerValidator.ValidateAsync(request);
        if (!isValid)
        {
            return TypedResponse<List<AnswerDTO>>.Failure(_getUserSurveyAnswerValidator.Notifications);
        }

        List<Answer> answers = await _answerRepository.GetUserSurveyAnswers(request.UserId, request.SurveyId);
        List<AnswerDTO> answerDTOs = _mapper.Map<List<AnswerDTO>>(answers);

        return TypedResponse<List<AnswerDTO>>.Success(answerDTOs);
    }

    public async Task<TypedResponse<StatisticsDTO>> GetAnswerStatistics(int surveyId)
    {
        bool isSurveyExist = await _surveyRepository.Exists(surveyId);
        if (!isSurveyExist)
        {
            var notifications = new List<Notification>
            {
                new Notification("SurveyId", $"Survey cannot be found with Id: {surveyId}")
            };
            return TypedResponse<StatisticsDTO>.Failure(notifications);
        }

        var questionStats = new List<QuestionStatisticsDTO>();

        var answers = await _answerRepository.GetAnswersBySurveyId(surveyId);
        if (!answers.IsNullOrEmpty())
        {
            var answerGroups = answers.GroupBy(a => new { a.Question, a.Department })
                .Select(g => new DepartmentAnswer()
                {
                    QuestionId = g.Key.Question.Id,
                    Department = g.Key.Department,
                    Texts = g.Key.Question.Texts,
                    Answers = g.ToList()
                })
                .ToList();
            var questions = answerGroups.Select(a => a.QuestionId).Distinct().ToList();

            foreach (var questionId in questions)
            {
                var departmentStats = new Dictionary<string, AnswerStatisticDTO>();
                var departments = Enum.GetValues(typeof(Department)).Cast<Department>().ToList();

                foreach (var department in departments)
                {
                    string departmentString = department.ToString();
                    var departmentAnswerGroup = answerGroups.FirstOrDefault(ag =>
                        ag.QuestionId == questionId &&
                        ag.Department.Equals(departmentString, StringComparison.OrdinalIgnoreCase));

                    if (departmentAnswerGroup != null && departmentAnswerGroup.Answers.Count > 0)
                    {
                        GetDepartmentStatistics(departmentAnswerGroup.Answers, departmentStats, departmentString);
                    }
                }

                Dictionary<string, string> texts = answerGroups
                    .FirstOrDefault(s => s.QuestionId.Equals(questionId))?.Texts;
                
                questionStats.Add(new QuestionStatisticsDTO
                    { QuestionId = questionId, Texts = texts ,DepartmentStats = departmentStats });
            }
        }

        return TypedResponse<StatisticsDTO>.Success(new StatisticsDTO
            { SurveyId = surveyId, QuestionStatistics = questionStats });
    }

    private static void GetDepartmentStatistics(List<Answer> answers, Dictionary<string, AnswerStatisticDTO> departmentStats,
        string departmentString)
    {
        var min = answers.Min(a => a.AnswerOption.Score);
        var max = answers.Max(a => a.AnswerOption.Score);
        var avg = answers.Average(a => a.AnswerOption.Score);

        departmentStats[departmentString] = new AnswerStatisticDTO { Min = min, Max = max, Avg = avg };
    }


    private async Task SaveAnswerWithOptionsAsync(PostUserAnswersRequest request, QuestionAnswerRequestDTO answer)
    {
        List<Answer> answers = answer.AnswerOptionIds
            .Select(answerOptionId => CreateAnswer(request, answer, answerOptionId)).ToList();
        await _answerRepository.AddRange(answers);
    }

    private async Task SaveAnswerWithTextAsync(PostUserAnswersRequest request, QuestionAnswerRequestDTO answer)
    {
        Answer newAnswer = CreateAnswer(request, answer);
        newAnswer.AnswerText = answer.AnswerText;
        await _answerRepository.Add(newAnswer);
    }

    private Answer CreateAnswer(PostUserAnswersRequest request, QuestionAnswerRequestDTO questionAnswer,
        int? answerOptionId = null)
    {
        return new Answer
        {
            SurveyId = request.SurveyId,
            QuestionId = questionAnswer.QuestionId,
            AnswerOptionId = answerOptionId,
            Department = request.Department,
            UserId = request.UserId,
            CreatedDate = DateTime.Now
        };
    }
}