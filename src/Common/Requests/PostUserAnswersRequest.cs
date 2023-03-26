using System.ComponentModel.DataAnnotations;

namespace Common.DTOs;

public class PostUserAnswersRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "SurveyId must be greater than 0")]
    public int UserId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "SurveyId must be greater than 0")]
    public int SurveyId { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    public List<QuestionAnswerRequestDTO> QuestionAnswers { get; set; }
}

