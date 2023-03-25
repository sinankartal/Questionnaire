namespace Common.DTOs;

public class QuestionAnswerDTO
{
    public int QuestionId { get; set; }

    public Dictionary<string, string> Texts { get; set; }

    public List<AnswerOptionDTO>? Answers { get; set; }

    public string? AnswerText { get; set; }
}