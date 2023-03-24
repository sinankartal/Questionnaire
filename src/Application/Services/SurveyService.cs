using System.Net;
using AutoMapper;
using Common;
using Common.DTOs;
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

    public async Task<List<SurveyDTO>> GetAll()
    {
        List<Survey> surveys = await _surveyRepository.GetAll();

        return _mapper.Map<List<SurveyDTO>>(surveys);
    }
}