using Application.Services;
using Application.Validator;
using AutoMapper;
using Common.DTOs;
using Common.Requests;
using FluentAssertions;
using Moq;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Test.services;

public class AnswerServiceTests
{
    private readonly Mock<IAnswerRepository> _answerRepositoryMock;
    private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
    private readonly Mock<ICustomValidator<PostUserAnswersRequest>> _processValidatorMock;
    private readonly Mock<ICustomValidator<GetUserSurveyAnswersRequest>> _getUserSurveyAnswerValidatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IAnswerService _answerService;

    public AnswerServiceTests()
    {
        _answerRepositoryMock = new Mock<IAnswerRepository>();
        _surveyRepositoryMock = new Mock<ISurveyRepository>();
        _processValidatorMock = new Mock<ICustomValidator<PostUserAnswersRequest>>();
        _getUserSurveyAnswerValidatorMock = new Mock<ICustomValidator<GetUserSurveyAnswersRequest>>();
        _mapperMock = new Mock<IMapper>();
        _answerService = new AnswerService(
            _answerRepositoryMock.Object,
            _processValidatorMock.Object,
            _surveyRepositoryMock.Object,
            _getUserSurveyAnswerValidatorMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task ProcessAsync_WithValidRequest_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new PostUserAnswersRequest
        {
            Id = 1,
            SurveyId = 1,
            UserId = 1,
            Department = "DEVELOPMENT",
            QuestionAnswers = new List<QuestionAnswerRequestDTO>
            {
                new QuestionAnswerRequestDTO
                {
                    QuestionId = 1,
                    AnswerOptionIds = new List<int> { 1 }
                }
            }
        };

        _processValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(true);


        // Act
        var result = await _answerService.ProcessAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Notifications.Should().BeEmpty();
        _answerRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<Answer>>()), Times.Once);
        _answerRepositoryMock.Verify(x => x.Add(It.IsAny<Answer>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidRequest_ShouldReturnFailureResponse()
    {
        // Arrange
        var request = new PostUserAnswersRequest
        {
            Id = 1,
            SurveyId = 1,
            UserId = 1,
            Department = "SALES",
            QuestionAnswers = new List<QuestionAnswerRequestDTO>
            {
                new QuestionAnswerRequestDTO
                {
                    QuestionId = 1,
                    AnswerText = "Test answer",
                    AnswerOptionIds = new List<int> { 1 }
                }
            }
        };

        _processValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(false);
        _processValidatorMock.Setup(x => x.Notifications)
            .Returns(new List<Notification> { new Notification("Test", "Invalid input") });

        // Act
        var result = await _answerService.ProcessAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Notifications.Should().ContainSingle().Which.Message.Should().Be("Invalid input");
        _answerRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<Answer>>()), Times.Never);
        _answerRepositoryMock.Verify(x => x.Add(It.IsAny<Answer>()), Times.Never);
    }

    [Fact]
    public async Task GetUserSurveyAnswers_WithValidRequest_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new GetUserSurveyAnswersRequest(1, 1);

        _getUserSurveyAnswerValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(true);

        var answerList = new List<Answer>
        {
            new Answer
            {
                Id = 1,
                UserId = 1,
                SurveyId = 1,
                Department = "SALES",
                QuestionId = 1,
                AnswerOptionId = 1,
                AnswerText = "Test answer",
                CreatedDate = DateTime.Now
            }
        };

        var answerDtos = new List<AnswerDTO>
        {
            new AnswerDTO
            {
                Department = "SALES",
                Id = 1,
                SurveyId = 1,
                UserId = 1,
                QuestionAnswers = new List<QuestionAnswerDTO>
                {
                    new QuestionAnswerDTO
                    {
                        Answers = new List<AnswerOptionDTO>
                        {
                            new AnswerOptionDTO
                            {
                                Id = 1,
                                OrderNumber = 1,
                                Texts = new Dictionary<string, string>()
                            }
                        },
                        QuestionId = 1,
                        Texts = new Dictionary<string, string>()
                    }
                }
            }
        };

        _answerRepositoryMock.Setup(x => x.GetUserSurveyAnswers(request.UserId, request.SurveyId))
            .ReturnsAsync(answerList);
        _mapperMock.Setup(x => x.Map<List<AnswerDTO>>(answerList)).Returns(answerDtos);

        // Act
        var result = await _answerService.GetUserSurveyAnswers(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Count.Should().Be(1);
        result.Data[0].UserId.Should().Be(1);
        result.Data[0].SurveyId.Should().Be(1);
        result.Data[0].Department.Should().Be("SALES");
        result.Data[0].QuestionAnswers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAnswerStatistics_WithValidSurveyId_ShouldReturnSuccessResponse()
    {
        // Arrange
        int surveyId = 1;

        _surveyRepositoryMock.Setup(x => x.Exists(surveyId)).ReturnsAsync(true);

        var question = new Question
        {
            Id = 1,
            SubjectId = 1,
            OrderNumber = 1,
            AnswerCategoryType = 1,
            TextsJson = "{}"
        };

        var answerOption = new AnswerOption
        {
            Id = 1,
            OrderNumber = 1,
            Question = question,
            QuestionId = 1,
            Score = 1,
            Texts = null
        };
        
        var answerList = new List<Answer>
        {
            new Answer
            {
                Id = 1,
                UserId = 1,
                SurveyId = 1,
                Department = "SALES",
                QuestionId = 1,
                AnswerOptionId = 1,
                CreatedDate = DateTime.Now,
                Question = question,
                AnswerOption = answerOption
            }
        };

        _answerRepositoryMock.Setup(x => x.GetAnswersBySurveyId(surveyId)).ReturnsAsync(answerList);

        var expectedDepartmentAnswers = new AnswerStatisticDTO
        {
            Avg = 1,
            Max = 1,
            Min = 1
        };

            // Act
        var result = await _answerService.GetAnswerStatistics(surveyId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.SurveyId.Should().Be(surveyId);
        result.Data.QuestionStatistics.Should().NotBeEmpty();
        result.Data.QuestionStatistics[0].DepartmentStats["SALES"].Should().BeEquivalentTo(expectedDepartmentAnswers);
    }

}