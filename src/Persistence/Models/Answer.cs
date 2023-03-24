namespace Persistence.Models;

public class Answer : BaseEntity
{
    public int UserId { get; set; }

    public string Department { get; set; }

    public int SurveyId { get; set; }

    public int QuestionId { get; set; }

    public int? AnswerOptionId { get; set; }

    public string? AnswerText { get; set; }

    public DateTime CreatedDate { get; set; }
    
    public Survey Survey { get; set; }
    public Question Question { get; set; }
    public AnswerOption AnswerOption { get; set; }
}