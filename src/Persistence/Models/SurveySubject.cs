namespace Persistence.Models;

public class SurveySubject : BaseEntity
{
    public int SurveyId { get; set; }

    public int SubjectId { get; set; }
    
    public int OrderNumber { get; set; }

    public Subject Subject { get; set; }
    public Survey Survey { get; set; }
}