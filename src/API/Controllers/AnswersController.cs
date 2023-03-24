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
public class AnswersController
{
    private readonly IAnswerService _answerService; 
    public AnswersController(IAnswerService answerService)
    {
        _answerService = answerService;
    }
    
    [HttpPost()]
    public async Task<ActionResult<GenericResponse>> ProcessAsync(PostUserAnswersRequest dto)
    {
        return await _answerService.ProcessAsync(dto);
    }
    
    [HttpGet("user/{userId}/survey/{surveyId}")]
    public async Task<ActionResult<GenericResponse>> GetUserSurveyAnswers(int userId, int surveyId)
    {
        return await _answerService.GetUserSurveyAnswers(new GetUserSurveyAnswersRequest(surveyId, userId));
    }
}