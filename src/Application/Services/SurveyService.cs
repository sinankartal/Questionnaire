using AutoMapper;
using Common.DTOs;
using Common.Responses;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Services;

public class SurveyService : ISurveyService
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IMapper _mapper;

    public SurveyService(ISurveyRepository surveyRepository, IMapper mapper)
    {
        _surveyRepository = surveyRepository;
        _mapper = mapper;
    }

    public async Task<TypedResponse<List<SurveyDTO>>> GetAll()
    {
        List<Survey> surveys = await _surveyRepository.GetAll();

        var dtos = _mapper.Map<List<SurveyDTO>>(surveys);

        return TypedResponse<List<SurveyDTO>>.Success(dtos);
    }
}