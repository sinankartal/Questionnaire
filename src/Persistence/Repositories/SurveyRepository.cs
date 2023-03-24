using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class SurveyRepository : Repository<Survey>, ISurveyRepository
{
    private readonly QuestionnaireDbContext _questionnaireDbContext;

    public SurveyRepository(QuestionnaireDbContext questionnaireDbContext) : base(questionnaireDbContext)
    {
        _questionnaireDbContext = questionnaireDbContext;
    }

    public Task<List<Survey>> GetAll()
    {
        return _questionnaireDbContext.Surveys.Include(s => s.SurveySubjects).ThenInclude(ss => ss.Subject)
            .ToListAsync();
    }
}