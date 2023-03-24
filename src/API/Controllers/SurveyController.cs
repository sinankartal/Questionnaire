using Application;
using Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SurveyController
{
    private readonly ISurveyService _surveyService; 
    public SurveyController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }
    
    [HttpGet("list")]
    public async Task<ActionResult<List<SurveyDTO>>> GetAll()
    {
        return await _surveyService.GetAll();
    }
}