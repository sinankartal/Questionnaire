namespace Common.DTOs;

public class AnswerOptionDTO
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public Dictionary<string, string> Texts { get; set; }
}