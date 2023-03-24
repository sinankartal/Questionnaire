using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class AnswerRepository : Repository<Answer>, IAnswerRepository
{
    private readonly QuestionnaireDbContext _questionnaireDbContext;

    public AnswerRepository(QuestionnaireDbContext questionnaireDbContext) : base(questionnaireDbContext)
    {
        _questionnaireDbContext = questionnaireDbContext;
    }

    public Task<List<Answer>> GetUserSurveyAnswers(int userId, int surveyId)
    {
        return  _questionnaireDbContext.Answers
            .Where(a => a.UserId == userId && a.SurveyId == surveyId)
            .Include(a => a.Survey)
            .Include(a => a.Question)
            .Include(a => a.AnswerOption)
            .ToListAsync();
    }
    
    public async Task<bool> UserHasCompletedSurvey(int userId, int surveyId)
    {
        return await _questionnaireDbContext.Answers.AnyAsync(answer => answer.UserId == userId && answer.SurveyId == surveyId);
    }
}