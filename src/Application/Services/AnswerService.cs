using Application.Validator;
using AutoMapper;
using Common.DTOs;
using Common.Requests;
using Common.Responses;
using Microsoft.IdentityModel.Tokens;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ICustomValidator<PostUserAnswersRequest> _processValidatior;
    private readonly ICustomValidator<GetUserSurveyAnswersRequest> _getUserSurveyAnswerValidator;
    private readonly IMapper _mapper;

    public AnswerService(IAnswerRepository answerRepository, ICustomValidator<PostUserAnswersRequest> processValidatior,
        ICustomValidator<GetUserSurveyAnswersRequest> getUserSurveyAnswerValidator, IMapper mapper)
    {
        _answerRepository = answerRepository;
        _processValidatior = processValidatior;
        _getUserSurveyAnswerValidator = getUserSurveyAnswerValidator;
        _mapper = mapper;
    }

    public async Task<GenericResponse> ProcessAsync(PostUserAnswersRequest dto)
    {
        bool isValid = await _processValidatior.ValidateAsync(dto);
        if (!isValid)
        {
            return GenericResponse.Failure(_processValidatior.Notifications);
        }

        foreach (var answer in dto.QuestionAnswers)
        {
            if (!answer.AnswerOptionIds.IsNullOrEmpty())
            {
                await SaveAnswerWithOptionsAsync(dto, answer);
            }
            else
            {
                await SaveAnswerWithTextAsync(dto, answer);
            }
        }

        return GenericResponse.Success();
    }

    public async Task<TypedGenericResponse<List<AnswerDTO>>> GetUserSurveyAnswers(GetUserSurveyAnswersRequest request)
    {
        bool isValid = await _getUserSurveyAnswerValidator.ValidateAsync(request);
        if (!isValid)
        {
            return TypedGenericResponse<List<AnswerDTO>>.Failure(_getUserSurveyAnswerValidator.Notifications);
        }

        List<Answer> answers = await _answerRepository.GetUserSurveyAnswers(request.UserId, request.SurveyId);
        List<AnswerDTO> answerDTOs = _mapper.Map<List<AnswerDTO>>(answers);

        return TypedGenericResponse<List<AnswerDTO>>.Success(answerDTOs);
    }

    private async Task SaveAnswerWithOptionsAsync(PostUserAnswersRequest request, QuestionAnswerRequestDTO answer)
    {
        List<Answer> answers = answer.AnswerOptionIds
            .Select(answerOptionId => CreateAnswer(request, answer, answerOptionId)).ToList();
        await _answerRepository.AddRange(answers);
    }

    private async Task SaveAnswerWithTextAsync(PostUserAnswersRequest request, QuestionAnswerRequestDTO answer)
    {
        Answer newAnswer = CreateAnswer(request, answer);
        newAnswer.AnswerText = answer.AnswerText;
        await _answerRepository.Add(newAnswer);
    }

    private Answer CreateAnswer(PostUserAnswersRequest request, QuestionAnswerRequestDTO questionAnswer, int? answerOptionId = null)
    {
        return new Answer
        {
            SurveyId = request.SurveyId,
            QuestionId = questionAnswer.QuestionId,
            AnswerOptionId = answerOptionId,
            Department = request.Department,
            UserId = request.UserId,
            CreatedDate = DateTime.Now
        };
    }
}