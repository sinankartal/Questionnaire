namespace Common.DTOs;

public class SurveyDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<SubjectDTO> Subjects { get; set; }
}