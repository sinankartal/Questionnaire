namespace Common.DTOs;

public class QuestionAnswerRequestDTO
{
    public int QuestionId { get; set; }

    public List<int>? AnswerOptionIds { get; set; }

    public string? AnswerText { get; set; }
}