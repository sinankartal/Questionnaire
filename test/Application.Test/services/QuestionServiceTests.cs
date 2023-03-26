using Application.Services;
using AutoMapper;
using Common.DTOs;
using FluentAssertions;
using Moq;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Test.services;

 public class QuestionServiceTests
    {
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
        private readonly Mock<ISubjectRepository> _subjectRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public QuestionServiceTests()
        {
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _surveyRepositoryMock = new Mock<ISurveyRepository>();
            _subjectRepositoryMock = new Mock<ISubjectRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetBySurveyIdPageable_WithValidSurveyId_ShouldReturnSuccessResponse()
        {
            // Arrange
            int surveyId = 1;
            int skip = 0;
            int limit = 10;

            _surveyRepositoryMock.Setup(x => x.Exists(surveyId)).ReturnsAsync(true);

            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    SubjectId = 1,
                    OrderNumber = 1,
                    AnswerCategoryType = 1,
                    TextsJson = "{}"
                }
            };

            var questionDTOs = new List<QuestionDTO>
            {
                new QuestionDTO
                {
                    Id = 1,
                    SubjectId = 1,
                    OrderNumber = 1,
                    AnswerCategoryType = "SomeType",
                    Texts = new Dictionary<string, string>()
                }
            };

            _questionRepositoryMock.Setup(x => x.GetBySurveyIdPageable(surveyId, skip, limit)).ReturnsAsync(questions);
            _mapperMock.Setup(x => x.Map<List<QuestionDTO>>(questions)).Returns(questionDTOs);

            var service = new QuestionService(_questionRepositoryMock.Object, _surveyRepositoryMock.Object, _subjectRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await service.GetBySurveyIdPageable(surveyId, skip, limit);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(questionDTOs);
        }

        [Fact]
        public async Task GetBySubjectIdPageable_WithValidSubjectId_ShouldReturnSuccessResponse()
        {
            // Arrange
            int subjectId = 1;
            int skip = 0;
            int limit = 10;

            _subjectRepositoryMock.Setup(x => x.Exists(subjectId)).ReturnsAsync(true);

            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    SubjectId = 1,
                    OrderNumber = 1,
                    AnswerCategoryType = 1,
                    TextsJson = "{}"
                }
            };

            var questionDTOs = new List<QuestionDTO>
            {
                new QuestionDTO
                {
                    Id = 1,
                    SubjectId = 1,
                    OrderNumber = 1,
                    AnswerCategoryType = "SomeType",
                    Texts = new Dictionary<string, string>()
                }
            };

            _questionRepositoryMock.Setup(x => x.GetBySubjectIdPageable(subjectId, skip, limit)).ReturnsAsync(questions);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<List<QuestionDTO>>(questions)).Returns(questionDTOs);

            var service = new QuestionService(_questionRepositoryMock.Object, _surveyRepositoryMock.Object, _subjectRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.GetBySubjectIdPageable(subjectId, skip, limit);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(questionDTOs);
        }

    }