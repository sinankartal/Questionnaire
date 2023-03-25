using Application;
using Common.DTOs;
using Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class QuestionsController
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("list/survey/{surveyId}")]
    public async Task<ActionResult<TypedResponse<List<QuestionDTO>>>> GetBySurveyIdPageable(int surveyId,
        [FromQuery] int skip = 0, [FromQuery] int limit = 10)
    {
        return await _questionService.GetBySurveyIdPageable(surveyId, skip, limit);
    }

    [HttpGet("list/subject/{subjectId}")]
    public async Task<ActionResult<TypedResponse<List<QuestionDTO>>>> GetBySubjectIdPageable(int subjectId,
        [FromQuery] int skip = 0, [FromQuery] int limit = 10)
    {
        return await _questionService.GetBySubjectIdPageable(subjectId, skip, limit);
    }
}