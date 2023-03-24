namespace Common.DTOs;

public class AnswerDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SurveyId { get; set; }

    public string Department { get; set; }

    public List<QuestionAnswerDTO> QuestionAnswers { get; set; }
}

