using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Persistence.Models;

public class Subject : BaseEntity
{
    public int OrderNumber { get; set; }
    public List<Question> Questions { get; set; }

    public List<SurveySubject> SurveySubjects { get; set; }
    
    public string TextsJson { get; set; }

    [NotMapped]
    public Dictionary<string, string> Texts
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, string>>(TextsJson);
        set => TextsJson = JsonConvert.SerializeObject(value);
    }
}