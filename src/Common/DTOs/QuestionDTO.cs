namespace Common.DTOs;

public class QuestionDTO
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int OrderNumber { get; set; }
    public string AnswerCategoryType { get; set; }
    public Dictionary<string, string> Texts { get; set; }
    public SubjectDTO Subject { get; set; }
    public List<AnswerOptionDTO> AnswerOptions { get; set; }
}