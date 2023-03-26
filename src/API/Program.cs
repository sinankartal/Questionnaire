using System.Text;
using API.JsonParse;
using API.Middlewares;
using Application;
using Application.AutoMapper;
using Application.Services;
using Application.Validator;
using AutoMapper;
using Common.DTOs;
using Common.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Persistence;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Questionnaire API", Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
//logger
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("../logs/api-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton(typeof(ILogger), Log.Logger);

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("TokenKey"))),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

//AutoMapper
var config = new MapperConfiguration(cfg => { cfg.AddProfile(new QuestionnaireProfile()); });
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddDbContext<QuestionnaireDbContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("db")));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();

builder.Services.AddScoped<ICustomValidator<PostUserAnswersRequest>, AnswerProcessValidator>();
builder.Services.AddScoped<ICustomValidator<GetUserSurveyAnswersRequest>, GetUserSurveyAnswersValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

AddSeedData(app);
app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void AddSeedData(WebApplication app)
{
    // MongoClient client = new MongoClient("mongodb+srv://sinankartal:1BES8wFlmTnXIF4l@cluster0.yc5bf.mongodb.net/?retryWrites=true&w=majority");
    
    var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetService<QuestionnaireDbContext>();

    string workingDirectory = Environment.CurrentDirectory;
    string filePath = workingDirectory + "/questionnaire.json";

    // Read the file contents into a string
    string json = File.ReadAllText(filePath);
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException("Could not find questionnaire.json file.");
    }

    // Deserialize the JSON string into an object
    Root data = JsonConvert.DeserializeObject<Root>(json);
    if (data == null)
    {
        throw new Exception("Error deserializing questionnaire.json file.");
    }

    var survey = new Survey() { Id = data.questionnaireId, Name = "Survey 1" };
    dbContext.Surveys.Add(survey);

    int i = 0;

    foreach (var subject in data.questionnaireItems)
    {
        var dbSubject = new Subject()
        {
            Id = subject.subjectId,
            OrderNumber = subject.orderNumber,
        };
        dbSubject.Texts = new Dictionary<string, string>
        {
            { "nl-NL", subject.texts.nlNL },
            { "en-US", subject.texts.enUS }
        };

        dbContext.Subjects.Add(dbSubject);
        

        foreach (var question in subject.questionnaireItems)
        {
            var dbQuestion = new Question()
            {
                Id = question.questionId,
                OrderNumber = question.orderNumber,
                AnswerCategoryType = question.answerCategoryType,
                Subject = dbSubject,
                SubjectId = dbSubject.Id
            };
        
            dbQuestion.Texts = new Dictionary<string, string>
            {
                { "nl-NL", question.texts.nlNL },
                { "en-US", question.texts.enUS }
            };
            dbContext.Questions.Add(dbQuestion);
            
        
            foreach (var answerOption in question.questionnaireItems)
            {
                if (answerOption.answerId is not null)
                {
                    var dbAnswerOption = new AnswerOption()
                    {
                        Id = (int)answerOption.answerId,
                        OrderNumber = answerOption.orderNumber,
                        Score = answerOption.orderNumber,
                        Question = dbQuestion,
                        QuestionId = dbQuestion.Id
                    };
                    dbAnswerOption.Texts = new Dictionary<string, string>
                    {
                        { "nl-NL", answerOption.texts.nlNL },
                        { "en-US", answerOption.texts.enUS }
                    };
                    dbContext.AnswerOptions.Add(dbAnswerOption);
                }
            }
        }
        
        dbContext.SurveySubjects.Add(new SurveySubject()
        {
            OrderNumber = i,
            Subject = dbSubject,
            Survey = survey
        });

        i++;
    }

    
    dbContext.SaveChanges();
}