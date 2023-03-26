using Application.Validator;
using AutoMapper;
using Common.DTOs;
using Common.Responses;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;

    public QuestionService(IQuestionRepository questionRepository, ISurveyRepository surveyRepository,
        ISubjectRepository subjectRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _surveyRepository = surveyRepository;
        _subjectRepository = subjectRepository;
        _mapper = mapper;
    }

    public async Task<TypedResponse<List<QuestionDTO>>> GetBySurveyIdPageable(int surveyId, int skip, int limit)
    {
        bool isSurveyExist = await _surveyRepository.Exists(surveyId);
        if (!isSurveyExist)
        {
            var notifications = new List<Notification>
            {
                new Notification("SurveyId", $"Survey cannot be found with Id: {surveyId}")
            };
            return TypedResponse<List<QuestionDTO>>.Failure(notifications);
        }

        List<Question> questions = await _questionRepository.GetBySurveyIdPageable(surveyId, skip, limit);

        var dtos = _mapper.Map<List<QuestionDTO>>(questions);

        return TypedResponse<List<QuestionDTO>>.Success(dtos);
    }

    public async Task<TypedResponse<List<QuestionDTO>>> GetBySubjectIdPageable(int subjectId, int skip,
        int limit)
    {
        bool isSubjectExist = await _subjectRepository.Exists(subjectId);
        if (!isSubjectExist)
        {
            var notifications = new List<Notification>
            {
                new Notification("SurveyId", $"Subject cannot be found with Id: {subjectId}")
            };
            return TypedResponse<List<QuestionDTO>>.Failure(notifications);
        }

        var questions = await _questionRepository.GetBySubjectIdPageable(subjectId, skip, limit);

        var dtos = _mapper.Map<List<QuestionDTO>>(questions);

        return TypedResponse<List<QuestionDTO>>.Success(dtos);
    }
}