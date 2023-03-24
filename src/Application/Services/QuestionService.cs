using System.Net;
using AutoMapper;
using Common;
using Common.DTOs;
using Persistence.IRepositories;

namespace Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;

    public QuestionService(IQuestionRepository questionRepository, ISurveyRepository surveyRepository, ISubjectRepository subjectRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _surveyRepository = surveyRepository;
        _subjectRepository = subjectRepository;
        _mapper = mapper;
    }

    public async Task<List<QuestionDTO>> GetBySurveyIdPageable(int surveyId, int skip, int limit)
    {
        bool isSurveyExist = await _surveyRepository.Exists(surveyId);
        if (!isSurveyExist)
        {
            throw new AppException($"Survey cannot be found with Id: {surveyId}", HttpStatusCode.NotFound);
        }
        var questions = await _questionRepository.GetBySurveyIdPageable(surveyId, skip, limit);

        return _mapper.Map<List<QuestionDTO>>(questions);
    }
    
    public async Task<List<QuestionDTO>> GetBySubjectIdPageable(int subjectId, int skip, int limit)
    {
        bool isSubjectExist = await _subjectRepository.Exists(subjectId);
        if (!isSubjectExist)
        {
            throw new AppException($"Subject cannot be found with Id: {subjectId}", HttpStatusCode.NotFound);
        }
        var questions = await _questionRepository.GetBySubjectIdPageable(subjectId, skip, limit);

        return _mapper.Map<List<QuestionDTO>>(questions);
    }
}
