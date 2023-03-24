using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using Newtonsoft.Json;

namespace Persistence.Models;

public class Question : BaseEntity
{
    public int SubjectId { get; set; }
    public int OrderNumber { get; set; }
    public int AnswerCategoryType { get; set; }
    public Subject Subject { get; set; }
    public List<AnswerOption>? AnswerOptions { get; set; }
    
    public string TextsJson { get; set; }

    [NotMapped]
    public Dictionary<string, string> Texts
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, string>>(TextsJson);
        set => TextsJson = JsonConvert.SerializeObject(value);
    }
}