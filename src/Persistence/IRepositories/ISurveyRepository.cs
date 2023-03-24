using Persistence.Models;

namespace Persistence.IRepositories;

public interface ISurveyRepository: IRepository<Survey>
{
    Task<List<Survey>> GetAll();
}