using Common.Requests;
using Persistence.IRepositories;

namespace Application.Validator;

public class GetUserSurveyAnswersValidator : ICustomValidator<GetUserSurveyAnswersRequest>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAnswerRepository _answerRepository;
    private List<Notification> _notifications;

    public GetUserSurveyAnswersValidator(ISurveyRepository surveyRepository, IAnswerRepository answerRepository)
    {
        _surveyRepository = surveyRepository;
        _answerRepository = answerRepository;
        _notifications = new List<Notification>();
    }

    public IReadOnlyList<Notification> Notifications => _notifications;

    public async Task<bool> ValidateAsync(GetUserSurveyAnswersRequest request)
    {
        bool surveyExists = await _surveyRepository.Exists(request.SurveyId);
        if (!surveyExists)
        {
            _notifications.Add(new Notification("SurveyId", $"Survey with Id {request.SurveyId} does not exist."));
        }
        
        // Check if the user has already completed the survey
        bool userHasCompletedSurvey = await _answerRepository.UserHasCompletedSurvey(request.UserId, request.SurveyId);
        if (!userHasCompletedSurvey)
        {
            _notifications.Add(new Notification("UserId", $"User with Id {request.UserId} has not completed the survey with Id {request.SurveyId}."));
        }
        
        return !_notifications.Any();
    }
}
