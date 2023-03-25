using Common.DTOs;
using Common.Responses;

namespace Application;

public interface IQuestionService
{
    Task<TypedResponse<List<QuestionDTO>>> GetBySurveyIdPageable(int surveyId, int skip, int limit);

    Task<TypedResponse<List<QuestionDTO>>> GetBySubjectIdPageable(int subjectId, int skip, int limit);
}