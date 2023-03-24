using Newtonsoft.Json;

namespace API.JsonParse;

public class QuestionnaireItem
{
    public int subjectId { get; set; }
    public int orderNumber { get; set; }
    public Texts texts { get; set; }
    public int itemType { get; set; }
    public List<QuestionnaireItem> questionnaireItems { get; set; }
    public int questionId { get; set; }
    public int answerCategoryType { get; set; }
    public int? answerId { get; set; }
    public int answerType { get; set; }
}

public class Root
{
    public int questionnaireId { get; set; }
    public List<QuestionnaireItem> questionnaireItems { get; set; }
}

public class Texts
{
    [JsonProperty("nl-NL")]
    public string nlNL { get; set; }

    [JsonProperty("en-US")]
    public string enUS { get; set; }
}