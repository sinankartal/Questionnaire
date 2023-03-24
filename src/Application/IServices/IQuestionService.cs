using Common.DTOs;

namespace Application;

public interface IQuestionService
{
    Task<List<QuestionDTO>> GetBySurveyIdPageable(int surveyId, int skip, int limit);

    Task<List<QuestionDTO>> GetBySubjectIdPageable(int subjectId, int skip, int limit);
}