using Application.Validator.IValidators;
using Common.Requests;
using Persistence.IRepositories;

namespace Application.Validator;

public class GetAnswerStatisticsValidator : IGetAnswerStatisticsValidator
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAnswerRepository _answerRepository;
    private List<Notification> _notifications;

    public GetAnswerStatisticsValidator(ISurveyRepository surveyRepository, IAnswerRepository answerRepository)
    {
        _surveyRepository = surveyRepository;
        _answerRepository = answerRepository;
        _notifications = new List<Notification>();
    }

    public IReadOnlyList<Notification> Notifications => _notifications;

    public async Task<bool> ValidateAsync(int surveyId)
    {
        bool surveyExists = await _surveyRepository.Exists(surveyId);
        if (!surveyExists)
        {
            _notifications.Add(new Notification("SurveyId", $"Survey with Id {surveyId} does not exist."));
        }
        
        var answersExist = await _answerRepository.ExistsBySurveyId(surveyId);
        if (!answersExist)
        {
            _notifications.Add(new Notification("SurveyId", $"No answer was found with Id {surveyId}."));
        }
        
        return !_notifications.Any();
    }
}
