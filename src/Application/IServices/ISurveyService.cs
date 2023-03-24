using Common.DTOs;

namespace Application;

public interface ISurveyService
{
    Task<List<SurveyDTO>> GetAll();
}