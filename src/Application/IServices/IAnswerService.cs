using Common.DTOs;
using Common.Requests;
using Common.Responses;

namespace Application;

public interface IAnswerService
{
    Task<Response> ProcessAsync(PostUserAnswersRequest dto);

    Task<TypedResponse<List<AnswerDTO>>> GetUserSurveyAnswers(GetUserSurveyAnswersRequest request);

    Task<TypedResponse<StatisticsDTO>> GetAnswerStatistics(int surveyId);
}