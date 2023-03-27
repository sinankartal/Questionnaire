using Common.DTOs;
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
    
    public async Task<List<DepartmentAnswer>> GetGroupedAnswersBySurveyId(int surveyId)
    {
        return await _dbContext.Answers
            .Include(a => a.AnswerOption)
            .Include(a => a.Question)
            .Where(a => a.SurveyId == surveyId && a.AnswerOptionId != null)
            .GroupBy(a => new { a.Question, a.Department })
            .Select(g => new DepartmentAnswer
            {
                QuestionId = g.Key.Question.Id,
                Department = g.Key.Department,
                Texts = g.Key.Question.Texts,
                Answers = g.ToList()
            })
            .ToListAsync();
    }

    public Task<bool> ExistsBySurveyId(int surveyId)
    {
        return _dbContext.Answers.AnyAsync(a => a.SurveyId.Equals(surveyId));
    }
}