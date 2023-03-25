using Persistence.Models;

namespace Common.DTOs;

public class DepartmentAnswer
{
    public int QuestionId { get; set; }

    public Dictionary<string, string> Texts { get; set; }
    public string Department { get; set; }
    public List<Answer> Answers { get; set; }
}
