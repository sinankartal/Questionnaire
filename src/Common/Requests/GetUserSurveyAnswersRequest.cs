namespace Common.Requests;

public class GetUserSurveyAnswersRequest
{
    public GetUserSurveyAnswersRequest(int surveyId, int userId)
    {
        SurveyId = surveyId;
        UserId = userId;
    }
    public int UserId { get; set; }

    public int SurveyId { get; set; }
}