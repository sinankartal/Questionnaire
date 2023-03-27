using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class AnswerOptionRepository : Repository<AnswerOption>, IAnswerOptionRepository
{
    private readonly QuestionnaireDbContext _questionnaireDbContext;

    public AnswerOptionRepository(QuestionnaireDbContext questionnaireDbContext) : base(questionnaireDbContext)
    {
        _questionnaireDbContext = questionnaireDbContext;
    }

    public async Task<bool> AreAllOptionIdsValidAsync(List<int> ids)
    {
        return await _questionnaireDbContext.AnswerOptions.Where(a => ids.Contains(a.Id)).CountAsync() == ids.Count;
    }
    
    
}