# Questionnaire API
This application provides a sophisticated and user-friendly web API for managing surveys, questions, and user answers. Designed for individuals responsible for survey management, the application offers a range of features that streamline the survey creation and monitoring process, such as:

Viewing a comprehensive list of existing surveys
Accessing a paginated list of questions for a specific survey or subject
Submitting user answers for designated survey questions
Reviewing a user's submitted answers for a particular survey
Retrieving aggregated answer statistics for insightful survey analysis
Leveraging the power of Azure, the application is hosted as a web app on the Azure portal and integrated with a GitHub repository. This seamless integration enables automatic code building and deployment upon each new merge to master branch. Although the application serves as an API, it also features Swagger documentation, which can be accessed at https://sk-questionnaire.azurewebsites.net/swagger/index.html for an interactive and visual representation of the API's endpoints.

The application boasts a robust design, employing an n-layer architecture to facilitate maintainability and scalability. Additional features that enhance the application's functionality and user experience include:

An error handler middleware to manage exceptions and provide consistent error responses
JWT token-based user authentication for secure access control
A notification pattern to streamline control flow
With its extensive feature set, elegant design, and seamless integration with cloud-based services, this application offers an efficient and intuitive solution for managing surveys, questions, and user answers.

## Configuration
SDK: The latest version of .NET 7 should be installed to run the application.
IDE: The application is developed using Rider. You can use this IDE or Visual Studio to open and run the application.
Database: The application uses an in-memory database, no additional setup is required to run the application.
Logging: The application uses serilog for logging, the configuration for serilog is already done.
Environment variables: Application does not use any environment variables.
Additional setup: The application does not require any additional setup.
Dependencies: The application depends on the latest version of the .NET 7 SDK and it will automatically install the required packages when you build the solution.
Configuration files: The application uses an appsettings.json configuration file to store settings such as database and other settings. This file is already included in the solution and no additional setup is required. If you need to make changes to the configuration, you can modify the appsettings.json file.

## Assumptions

The questionnaire.json file serves as sample data for the application.
Each answer option has a score value that corresponds to its order number in the sequence.
User validation is not required; any integer provided for userId will be accepted by the system as a valid user.
A user can submit answers multiple times using the same or different requests.

## Controller Endpoints

AnswersController:
This controller is responsible for managing user answers to survey questions. It contains three endpoints:

POST /Answers: Processes the user's answers provided in the PostUserAnswersRequest DTO.
GET /Answers/user/{userId}/survey/{surveyId}: Retrieves the answers submitted by a user (specified by userId) for a specific survey (specified by surveyId).
GET /Answers/statistics/survey/{surveyId}: Retrieves the aggregated statistics of answers for a specific survey (specified by surveyId).

QuestionsController:
This controller is responsible for managing survey questions. It contains two endpoints:

GET /Questions/list/survey/{surveyId}: Retrieves a list of questions for a specific survey (specified by surveyId) with pagination support, controlled by the skip and limit query parameters.
GET /Questions/list/subject/{subjectId}: Retrieves a list of questions for a specific subject (specified by subjectId) with pagination support, controlled by the skip and limit query parameters.

SurveyController:
This controller is responsible for managing surveys. It contains a single endpoint:

GET /Survey/list: Retrieves a list of all surveys.

TokenController:
This controller is responsible for managing authentication tokens. It contains a single endpoint:

POST /Token/login: Authenticates the user using the provided username and password. If the credentials are valid, a token is generated and returned as a JSON object.

## Execution
🌟 Getting Started with the Questionnaire 🌟
Follow these easy steps to get the application up and running in no time!

A. Access the Hosted Application 🌐

The application is hosted on the following URL:
https://sk-questionnaire.azurewebsites.net/swagger/index.html

B. Run the application on local machine

1.Clone the Repository 🚀
Begin by cloning the GitHub repository using the provided link.

git clone <repository_link>
2. Navigate to the API Folder 📁
Next, move to the "API" folder, which is located within the "src" folder:

cd src/API
3. Build and Run the Application 🏗️
Execute the following commands to build and run the application:

dotnet build
dotnet run

4. Local Machine API Hosting 🖥️
By default, the API will be hosted on your local machine at:
http://localhost:5064
Note: The port number 5064 is specified in the launchSettings.json file.

5. Explore and Test the API 🔍
Visit http://localhost:5064 in your browser to view the available endpoints and test the API using Swagger.

That's it! You're all set to start using the Questionnaire! 🎉

