namespace Common.DTOs;

public class PostUserAnswersRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SurveyId { get; set; }

    public string Department { get; set; }

    public List<QuestionAnswerRequestDTO> QuestionAnswers { get; set; }
}

