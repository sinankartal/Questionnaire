using Common.DTOs;
using Common.Requests;
using Common.Responses;

namespace Application;

public interface IAnswerService
{
    Task<GenericResponse> ProcessAsync(PostUserAnswersRequest dto);

    Task<TypedGenericResponse<List<AnswerDTO>>> GetUserSurveyAnswers(GetUserSurveyAnswersRequest request);
}