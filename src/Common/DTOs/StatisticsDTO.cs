namespace Common.DTOs;

public class StatisticsDTO
{
    public int SurveyId { get; set; }
    public List<QuestionStatisticsDTO> QuestionStatistics { get; set; }
}

public class QuestionStatisticsDTO
{
    public int QuestionId { get; set; }

    public Dictionary<string, string> Texts { get; set; }
    public Dictionary<string, AnswerStatisticDTO> DepartmentStats { get; set; }
}

public class AnswerStatisticDTO
{
    public int Min { get; set; }
    public int Max { get; set; }
    public double Avg { get; set; }
}
