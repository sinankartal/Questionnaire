using Application.Validator;
using Application.Validator.IValidators;
using AutoMapper;
using Common.DTOs;
using Common.Requests;
using Common.Responses;
using Microsoft.IdentityModel.Tokens;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ICustomValidator<PostUserAnswersRequest> _processValidatior;
    private readonly ICustomValidator<GetUserSurveyAnswersRequest> _getUserSurveyAnswerValidator;
    private readonly IGetAnswerStatisticsValidator _getAnswerStatisticsValidator;
    private readonly IMapper _mapper;

    public AnswerService(IAnswerRepository answerRepository, ICustomValidator<PostUserAnswersRequest> processValidatior,
        ICustomValidator<GetUserSurveyAnswersRequest> getUserSurveyAnswerValidator,
        IGetAnswerStatisticsValidator getAnswerStatisticsValidator, IMapper mapper)
    {
        _answerRepository = answerRepository;
        _processValidatior = processValidatior;
        _getUserSurveyAnswerValidator = getUserSurveyAnswerValidator;
        _getAnswerStatisticsValidator = getAnswerStatisticsValidator;
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
        bool isValid = await _getAnswerStatisticsValidator.ValidateAsync(surveyId);
        if (!isValid)
        {
            return TypedResponse<StatisticsDTO>.Failure(_getAnswerStatisticsValidator.Notifications);
        }

        var answerGroups = await _answerRepository.GetGroupedAnswersBySurveyId(surveyId);

        var questionStats = GetQuestionStatistics(answerGroups);
        return TypedResponse<StatisticsDTO>.Success(new StatisticsDTO
            { SurveyId = surveyId, QuestionStatistics = questionStats });
    }
    private List<QuestionStatisticsDTO> GetQuestionStatistics(List<DepartmentAnswer> answerGroups)
    {
        var questionIds = answerGroups.Select(a => a.QuestionId).Distinct().ToList();
        var questionStats = new List<QuestionStatisticsDTO>();

        foreach (var questionId in questionIds)
        {
            var departmentStats = GetDepartmentStatisticsForQuestion(answerGroups, questionId);
            var texts = answerGroups.FirstOrDefault(s => s.QuestionId.Equals(questionId))?.Texts;

            questionStats.Add(new QuestionStatisticsDTO
                { QuestionId = questionId, Texts = texts, DepartmentStats = departmentStats });
        }

        return questionStats;
    }

    private Dictionary<string, AnswerStatisticDTO> GetDepartmentStatisticsForQuestion(
        List<DepartmentAnswer> answerGroups, int questionId)
    {
        var departmentStats = new Dictionary<string, AnswerStatisticDTO>();
        var departments = answerGroups.Select(s => s.Department).Distinct().ToList();

        foreach (var department in departments)
        {
            var departmentAnswerGroup = answerGroups.FirstOrDefault(ag =>
                ag.QuestionId == questionId &&
                ag.Department.Equals(department, StringComparison.OrdinalIgnoreCase));

            if (departmentAnswerGroup != null && departmentAnswerGroup.Answers.Count > 0)
            {
                GetDepartmentStatistics(departmentAnswerGroup.Answers, departmentStats, department);
            }
        }

        return departmentStats;
    }

    private static void GetDepartmentStatistics(List<Answer> answers,
        Dictionary<string, AnswerStatisticDTO> departmentStats,
        string department)
    {
        var min = answers.Min(a => a.AnswerOption.Score);
        var max = answers.Max(a => a.AnswerOption.Score);
        var avg = Math.Round(answers.Average(a => a.AnswerOption.Score), 2);

        departmentStats[department] = new AnswerStatisticDTO { Min = min, Max = max, Avg = avg };
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