using Common.DTOs;
using Persistence.Models;

namespace Persistence.IRepositories;

public interface IAnswerRepository: IRepository<Answer>
{
    Task<List<Answer>> GetUserSurveyAnswers(int userId, int surveyId);

    Task<bool> UserHasCompletedSurvey(int userId, int surveyId);
}