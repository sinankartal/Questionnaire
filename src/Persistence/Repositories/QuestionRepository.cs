using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class QuestionRepository : Repository<Question>, IQuestionRepository
{
    private readonly QuestionnaireDbContext _questionnaireDbContext;

    public QuestionRepository(QuestionnaireDbContext questionnaireDbContext) : base(questionnaireDbContext)
    {
        _questionnaireDbContext = questionnaireDbContext;
    }

    public Task<List<Question>> GetBySurveyIdPageable(int surveyId, int skip, int limit)
    {
        return _questionnaireDbContext.SurveySubjects
            .Where(ss => ss.SurveyId == surveyId)
            .SelectMany(ss => ss.Subject.Questions)
            .Include(q => q.Subject)
            .Include(q => q.AnswerOptions)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();
    }

    public Task<List<Question>> GetBySubjectIdPageable(int subjectId, int skip, int limit)
    {
        return _questionnaireDbContext.Subjects
            .Where(ss => ss.Id == subjectId)
            .Include(s => s.Questions)
            .ThenInclude(q => q.AnswerOptions).AsNoTracking()
            .SelectMany(s => s.Questions)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();
    }
    
    public Task<Question> GetWithAnswerOptionsAsync(int id)
    {
        return _questionnaireDbContext.Questions.Include(q=>q.AnswerOptions).FirstOrDefaultAsync(s=>s.Id.Equals(id));
    }
}