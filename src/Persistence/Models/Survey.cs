
namespace Persistence.Models;

public class Survey : BaseEntity
{ 
    public string Name { get; set; }
    public List<SurveySubject> SurveySubjects { get; set; }
}