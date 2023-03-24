namespace Common.DTOs;

public class QuestionAnswerDTO
{
    public int QuestionId { get; set; }

    public List<AnswerOptionDTO>? AnswerOptions { get; set; }

    public string? AnswerText { get; set; }
}