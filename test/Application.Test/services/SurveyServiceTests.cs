using Application.Services;
using AutoMapper;
using Common.DTOs;
using FluentAssertions;
using Moq;
using Persistence.IRepositories;
using Persistence.Models;

namespace Application.Test.services;

public class SurveyServiceTests
{
    private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ISurveyService _surveyService;

    public SurveyServiceTests()
    {
        _surveyRepositoryMock = new Mock<ISurveyRepository>();
        _mapperMock = new Mock<IMapper>();
        _surveyService = new SurveyService(_surveyRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnSuccessResponseWithListOfSurveys()
    {
        // Arrange
        var surveys = new List<Survey>
        {
            new Survey
            {
                Id = 1,
                Name = "Survey 1",
                SurveySubjects = new List<SurveySubject>()
            }
        };

        var surveyDTOs = new List<SurveyDTO>
        {
            new SurveyDTO
            {
                Id = 1,
                Name = "Survey 1",
                Subjects = new List<SubjectDTO>()
            }
        };

        _surveyRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(surveys);
        _mapperMock.Setup(x => x.Map<List<SurveyDTO>>(surveys)).Returns(surveyDTOs);
        
        // Act
        var result = await _surveyService.GetAll();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(surveyDTOs);
    }
}