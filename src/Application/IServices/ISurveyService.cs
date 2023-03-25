using Common.DTOs;
using Common.Responses;

namespace Application;

public interface ISurveyService
{
    Task<TypedResponse<List<SurveyDTO>>> GetAll();
}