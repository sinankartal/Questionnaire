using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Persistence.Models;

public class AnswerOption : BaseEntity
{
    public int  QuestionId { get; set; }

    public int OrderNumber { get; set; }
    
    public int Score { get; set; }

    public Question Question { get; set; }
    public string TextsJson { get; set; }

    [NotMapped]
    public Dictionary<string, string> Texts
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, string>>(TextsJson);
        set => TextsJson = JsonConvert.SerializeObject(value);
    }
}