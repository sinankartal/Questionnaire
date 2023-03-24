using Persistence.Models;

namespace Persistence.IRepositories;

public interface IQuestionRepository: IRepository<Question>
{
    Task<List<Question>> GetBySurveyIdPageable(int surveyId, int skip, int limit);
    
    Task<List<Question>> GetBySubjectIdPageable(int subjectId, int skip, int limit);
}