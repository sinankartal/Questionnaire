using AutoMapper;
using Common.DTOs;
using Common.Enums;
using Persistence.Models;

namespace Application.AutoMapper;

public class QuestionnaireProfile : Profile
{
    public QuestionnaireProfile()
    {
        CreateMap<AnswerOption, AnswerOptionDTO>()
            .ForMember(dest => dest.Texts, opt => opt.MapFrom(src => src.Texts));

        CreateMap<Question, QuestionDTO>()
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.AnswerOptions, opt => opt.MapFrom(src => src.AnswerOptions))
            .ForMember(dest => dest.AnswerCategoryType, opt => opt.MapFrom(src => Enum.GetName(typeof(AnswerCategoryType), src.AnswerCategoryType)))
            .ForMember(dest => dest.Texts, opt => opt.MapFrom(src => src.Texts));

        CreateMap<Subject, SubjectDTO>()
            .ForMember(dest => dest.Texts, opt => opt.MapFrom(src => src.Texts));

        CreateMap<Survey, SurveyDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.SurveySubjects.Select(ss => ss.Subject)));
        
        CreateMap<Answer, AnswerDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(src => src.SurveyId))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
            .ForMember(dest => dest.QuestionAnswers, opt => opt.MapFrom(src => new List<QuestionAnswerDTO> { new QuestionAnswerDTO
            {
                QuestionId = src.QuestionId,
                Texts = src.Question.Texts,
                Answers = src.AnswerOption != null ? new List<AnswerOptionDTO> { new AnswerOptionDTO
                {
                    Id = src.AnswerOption.Id,
                    OrderNumber = src.AnswerOption.OrderNumber,
                    Texts = src.AnswerOption.Texts
                }} : null,
                AnswerText = src.AnswerText,
            }}));

    }
}