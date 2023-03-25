using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class AnswerRepository : Repository<Answer>, IAnswerRepository
{
    private readonly QuestionnaireDbContext _dbContext;

    public AnswerRepository(QuestionnaireDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Answer>> GetUserSurveyAnswers(int userId, int surveyId)
    {
        return  _dbContext.Answers
            .Where(a => a.UserId == userId && a.SurveyId == surveyId)
            .Include(a => a.Survey)
            .Include(a => a.Question)
            .Include(a => a.AnswerOption)
            .ToListAsync();
    }
    
    public Task<bool> UserHasCompletedSurvey(int userId, int surveyId)
    {
        return _dbContext.Answers.AnyAsync(answer => answer.UserId == userId && answer.SurveyId == surveyId);
    }
    
    public  Task<List<Answer>> GetAnswersBySurveyId(int surveyId)
    {
        return _dbContext.Answers
            .Include(a => a.AnswerOption)
            .Include(s=>s.Question)
            .Where(a => a.SurveyId == surveyId && a.AnswerOptionId != null)
            .ToListAsync();
        
    }


}