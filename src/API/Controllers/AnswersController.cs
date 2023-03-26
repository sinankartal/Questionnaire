using Application;
using Common.DTOs;
using Common.Requests;
using Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AnswersController : ControllerBase
{
    private readonly IAnswerService _answerService; 
    public AnswersController(IAnswerService answerService)
    {
        _answerService = answerService;
    }
    
    [HttpPost()]
    public async Task<ActionResult<Response>> ProcessAsync(PostUserAnswersRequest dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await _answerService.ProcessAsync(dto);
    }
    
    [HttpGet("user/{userId}/survey/{surveyId}")]
    public async Task<ActionResult<Response>> GetUserSurveyAnswers(int userId, int surveyId)
    {
        if (userId <= 0 || surveyId <= 0)
        {
            return BadRequest("Both userId and surveyId must be positive, non-zero values.");
        }
        return await _answerService.GetUserSurveyAnswers(new GetUserSurveyAnswersRequest(surveyId, userId));
    }
    
    [HttpGet("statistics/survey/{surveyId}")]
    public async Task<ActionResult<TypedResponse<StatisticsDTO>>> GetAnswerStatistics(int surveyId)
    {
        if (surveyId <= 0)
        {
            return BadRequest("SurveyId must be positive, non-zero values.");
        }
        
        return await _answerService.GetAnswerStatistics(surveyId);
    }
}