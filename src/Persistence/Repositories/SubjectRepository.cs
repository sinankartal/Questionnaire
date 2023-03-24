using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(QuestionnaireDbContext questionnaireDbContext) : base(questionnaireDbContext)
    {
    }
}