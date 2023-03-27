using Persistence.Models;

namespace Persistence.IRepositories;

public interface IAnswerOptionRepository: IRepository<AnswerOption>
{
    Task<bool> AreAllOptionIdsValidAsync(List<int> ids);
}