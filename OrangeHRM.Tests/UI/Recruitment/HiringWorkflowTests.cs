using System.Text.Json;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using log4net;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Recruitment;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Helpers;
using OrangeHRM.Tests.Infrastructure;

namespace OrangeHRM.Tests.UI.Recruitment;

[TestFixture]
public class RecruitmentWorkflowTests : UiTestBase
{
    private const int JobTitleId = 22;
    private const string JobTitle = "Account Assistant";
    private const string CvFileName = "candidate-cv.pdf";

    private static readonly ILog Log =
        LogManager.GetLogger(typeof(RecruitmentWorkflowTests));

    private static void RunStep(string name, Action action)
    {
        AllureApi.Step(name, action);
    }

    private static T RunStep<T>(string name, Func<T> action)
    {
        return AllureApi.Step(name, action);
    }

    [SetUp]
    public void Login()
    {
        Log.Info("Recruitment workflow setup: logging in to OrangeHRM.");

        var loginPage = RunStep(
            "Create LoginPage object",
            () => new LoginPage());

        var dashboardPage = RunStep(
            "Log in with configured valid credentials",
            () => loginPage.Login(
                TestSettings.Username,
                TestSettings.Password));

        RunStep("Verify Dashboard is displayed", () =>
        {
            Assert.That(
                dashboardPage.IsDisplayed(),
                Is.True,
                "Dashboard page was not displayed after login.");
        });

        Log.Info("Recruitment workflow setup: login completed successfully.");
    }

    [Test]
    [AllureName("Recruitment Workflow - candidate is hired successfully")]
    [AllureTag("UI", "API", "EndToEnd", "HiringWorkflow")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Executes the end-to-end recruitment workflow. Creates a hiring manager and vacancy through the API, creates a candidate through the UI with an uploaded CV, moves the candidate through Shortlisted, Interview Scheduled, Interview Passed, and Job Offered, and verifies the final Hired status. Attempts to clean up the candidate, vacancy, and employee.")]
    public void RecruitmentWorkflow_CandidateIsHiredSuccessfully()
    {
        Log.Info(
            "Test started: RecruitmentWorkflow_CandidateIsHiredSuccessfully.");

        var runId = Guid.NewGuid().ToString("N")[..10];

        var employeeFirstName = $"Manager{runId}";
        var employeeLastName = "E2E";
        var employeeName = $"{employeeFirstName} {employeeLastName}";
        var vacancyName = $"E2E Vacancy {runId}";

        var candidateFirstName = $"Candidate{runId}";
        var candidateMiddleName = "E2E";
        var candidateLastName = "Automation";
        var candidateEmail = $"candidate.{runId}@example.com";

        Log.Info($"Generated unique test data. Run ID: {runId}.");
        Log.Info($"Candidate: {candidateFirstName} {candidateLastName}.");
        Log.Info($"Hiring manager: {employeeName}.");
        Log.Info($"Vacancy: {vacancyName}.");

        var cvFilePath = RunStep(
            "Resolve candidate CV file path",
            () => Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "TestData",
                CvFileName));

        Log.Info($"Checking that the CV file exists: {CvFileName}.");

        RunStep("Verify candidate CV file exists", () =>
        {
            Assert.That(
                File.Exists(cvFilePath),
                Is.True,
                $"Candidate CV was not found: {cvFilePath}");
        });

        Log.Info("CV file exists.");

        Log.Info("Reading the OrangeHRM API access token from the environment.");

        var accessToken = RunStep(
            "Read API access token from environment",
            () => Environment.GetEnvironmentVariable(
                "ORANGEHRM_ACCESS_TOKEN"));

        RunStep("Verify API access token is configured", () =>
        {
            Assert.That(
                accessToken,
                Is.Not.Null.And.Not.Empty,
                "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");
        });

        Log.Info("API access token is available.");

        using var employeesApi = new EmployeesApiClient(accessToken!);
        using var vacanciesApi = new VacanciesApiClient(accessToken!);
        using var candidatesApi = new CandidatesApiClient(accessToken!);

        int? employeeId = null;
        int? vacancyId = null;

        try
        {
            // Create the hiring manager through the API.
            Log.Info("Creating the hiring manager through the API.");

            var employeeRequest = RunStep(
                "Prepare hiring manager request",
                () => new CreateEmployeeRequest
                {
                    FirstName = employeeFirstName,
                    LastName = employeeLastName,
                    MiddleName = string.Empty
                });

            var employeeResponse = RunStep(
                "Create hiring manager through API",
                () => employeesApi.CreateEmployee(employeeRequest));

            Log.Info(
                $"Employee creation API response received. " +
                $"HTTP status: {(int)employeeResponse.StatusCode}.");

            RunStep("Verify hiring manager creation response", () =>
            {
                Assert.That(
                    employeeResponse.IsSuccessful,
                    Is.True,
                    $"Employee creation failed. HTTP: " +
                    $"{(int)employeeResponse.StatusCode}. " +
                    $"Body: {employeeResponse.Content}");
            });

            var createdEmployee = RunStep(
                "Deserialize hiring manager response",
                () => JsonSerializer.Deserialize<CreateEmployeeResponse>(
                    employeeResponse.Content!));

            RunStep("Verify hiring manager information", () =>
            {
                Assert.That(createdEmployee?.Data, Is.Not.Null);
            });

            employeeId = createdEmployee!.Data!.EmpNumber;

            RunStep("Verify hiring manager ID and name", () =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(employeeId.Value, Is.GreaterThan(0));

                    Assert.That(
                        createdEmployee.Data.FirstName,
                        Is.EqualTo(employeeFirstName));

                    Assert.That(
                        createdEmployee.Data.LastName,
                        Is.EqualTo(employeeLastName));
                });
            });

            Log.Info(
                $"Hiring manager created and verified. Employee ID: {employeeId.Value}.");

            // Create the vacancy through the API.
            Log.Info($"Creating vacancy through the API: {vacancyName}.");

            var vacancyRequest = RunStep(
                "Prepare vacancy request",
                () => new CreateVacancyRequest
                {
                    Name = vacancyName,
                    Status = true,
                    JobTitleId = JobTitleId,
                    IsPublished = true,
                    Description = "Vacancy created for the E2E recruitment workflow.",
                    NumOfPositions = 1,
                    EmployeeId = employeeId.Value
                });

            var vacancyResponse = RunStep(
                "Create vacancy through API",
                () => vacanciesApi.CreateVacancy(vacancyRequest));

            Log.Info(
                $"Vacancy creation API response received. " +
                $"HTTP status: {(int)vacancyResponse.StatusCode}.");

            RunStep("Verify vacancy creation response", () =>
            {
                Assert.That(
                    vacancyResponse.IsSuccessful,
                    Is.True,
                    $"Vacancy creation failed. HTTP: " +
                    $"{(int)vacancyResponse.StatusCode}. " +
                    $"Body: {vacancyResponse.Content}");
            });

            var createdVacancy = RunStep(
                "Deserialize created vacancy response",
                () => JsonSerializer.Deserialize<CreateVacancyResponse>(
                    vacancyResponse.Content!));

            RunStep("Verify created vacancy data is present", () =>
            {
                Assert.That(createdVacancy?.Data, Is.Not.Null);
            });

            var vacancyData = createdVacancy!.Data!;
            vacancyId = vacancyData.Id;

            RunStep("Verify vacancy fields and hiring manager", () =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(vacancyId.Value, Is.GreaterThan(0));

                    Assert.That(
                        vacancyData.Name,
                        Is.EqualTo(vacancyName));

                    Assert.That(
                        vacancyData.JobTitle?.Id,
                        Is.EqualTo(JobTitleId));

                    Assert.That(
                        vacancyData.JobTitle?.Title,
                        Is.EqualTo(JobTitle));

                    Assert.That(
                        vacancyData.HiringManager?.Id,
                        Is.EqualTo(employeeId.Value),
                        "Vacancy is linked to the wrong Hiring Manager.");

                    Assert.That(
                        vacancyData.HiringManager?.FirstName,
                        Is.EqualTo(employeeFirstName));

                    Assert.That(
                        vacancyData.HiringManager?.LastName,
                        Is.EqualTo(employeeLastName));
                });
            });

            Log.Info(
                $"Vacancy created and verified. Vacancy ID: {vacancyId.Value}.");

            // Open the Recruitment section in the UI.
            Log.Info("Navigating to the OrangeHRM dashboard.");

            RunStep("Navigate to Dashboard", () =>
            {
                DriverManager.Driver.Navigate().GoToUrl(
                    new Uri(
                        new Uri(TestSettings.BaseUrl),
                        "web/index.php/dashboard/index").ToString());
            });

            var dashboardPage = RunStep(
                "Create DashboardPage object",
                () => new DashboardPage());

            Log.Info("Opening the Recruitment section.");

            RunStep(
                "Open Recruitment section",
                () => dashboardPage.SidePanel.OpenRecruitment());

            var recruitmentPage = RunStep(
                "Create RecruitmentPage object",
                () => new RecruitmentPage());

            RunStep("Verify Recruitment page is displayed", () =>
            {
                Assert.That(
                    recruitmentPage.IsDisplayed(),
                    Is.True,
                    "Recruitment page was not displayed.");
            });

            Log.Info("Recruitment page is displayed.");

            // Create the candidate through the UI.
            Log.Info("Opening the candidates list.");

            var candidatesPage = RunStep(
                "Open candidates list",
                () => recruitmentPage.OpenCandidates());

            Log.Info("Opening the Add Candidate form.");

            var addCandidatePage = RunStep(
                "Open Add Candidate form",
                () => candidatesPage.ClickAdd());

            Log.Info("Filling in the candidate form.");

            RunStep("Fill in candidate information", () =>
            {
                addCandidatePage.FillCandidateForm(
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail);
            });

            Log.Info($"Selecting vacancy: {vacancyName}.");

            RunStep(
                "Select vacancy for candidate",
                () => addCandidatePage.SelectVacancy(vacancyName));

            Log.Info($"Uploading CV file: {CvFileName}.");

            RunStep(
                "Upload candidate CV",
                () => addCandidatePage.UploadCv(cvFilePath));

            Log.Info("Checking that the CV is selected in the form.");

            RunStep("Verify CV is selected", () =>
            {
                Assert.That(
                    addCandidatePage.IsCvSelected(CvFileName),
                    Is.True,
                    "CV was not selected in the Add Candidate form.");
            });

            Log.Info("CV is selected. Saving the candidate.");

            var candidateDetailsPage = RunStep(
                "Save candidate",
                () => addCandidatePage.Save());

            Log.Info(
                "Verifying candidate details after creation. Expected status: Application Initiated.");

            RunStep(
                "Verify candidate profile: Application Initiated",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Application Initiated"));

            Log.Info("Candidate created and verified.");

            // Shortlist the candidate.
            Log.Info("Moving the candidate to the Shortlisted stage.");

            var shortlistPage = RunStep(
                "Open Shortlist Candidate form",
                () => candidateDetailsPage.ShortlistButtonClick());

            RunStep("Verify Shortlist Candidate page is displayed", () =>
            {
                Assert.That(
                    shortlistPage.IsDisplayed(),
                    Is.True,
                    "Shortlist Candidate page was not displayed.");
            });

            Log.Info("Shortlist Candidate page is displayed. Saving the stage.");

            candidateDetailsPage = RunStep(
                "Save Shortlisted stage",
                () => shortlistPage.SaveButtonClick());

            Log.Info("Verifying candidate details after shortlisting.");

            RunStep(
                "Verify candidate profile: Shortlisted",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Shortlisted"));

            Log.Info("Candidate successfully reached the Shortlisted stage.");

            // Schedule the interview.
            Log.Info("Opening the Schedule Interview form.");

            var scheduleInterviewPage = RunStep(
                "Open Schedule Interview form",
                () => candidateDetailsPage.ClickScheduleInterviewButton());

            RunStep("Verify Schedule Interview page is displayed", () =>
            {
                Assert.That(
                    scheduleInterviewPage.IsDisplayed(),
                    Is.True,
                    "Schedule Interview page was not displayed.");
            });

            Log.Info($"Selecting interviewer: {employeeName}.");

            RunStep(
                "Select hiring manager as interviewer",
                () => scheduleInterviewPage.SelectInterviewer(employeeName));

            RunStep("Verify selected interviewer", () =>
            {
                Assert.That(
                    scheduleInterviewPage.GetInterviewer(),
                    Is.EqualTo(employeeName),
                    "The selected interviewer does not match the created employee.");
            });

            Log.Info("Interviewer selection verified.");

            var interviewDate = DateTime.Today.AddDays(1);
            var interviewTitle = $"Technical Interview {runId}";

            Log.Info($"Selecting interview date: {interviewDate:yyyy-MM-dd}.");

            RunStep(
                "Select interview date",
                () => scheduleInterviewPage.SelectDate(interviewDate));

            Log.Info($"Entering interview title: {interviewTitle}.");

            RunStep(
                "Enter interview title",
                () => scheduleInterviewPage.EnterInterviewTitle(interviewTitle));

            Log.Info("Saving the scheduled interview.");

            candidateDetailsPage = RunStep(
                "Save scheduled interview",
                () => scheduleInterviewPage.SaveButtonClick());

            Log.Info("Verifying candidate status after scheduling the interview.");

            RunStep(
                "Verify candidate profile: Interview Scheduled",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Interview Scheduled"));

            Log.Info("Candidate successfully reached the Interview Scheduled stage.");

            // Mark the interview as passed.
            Log.Info("Opening the Mark Interview Passed form.");

            var markInterviewPassedPage = RunStep(
                "Open Mark Interview Passed form",
                () => candidateDetailsPage.ClickMarkInterviewPassedButton());

            Log.Info("Saving the interview result.");

            candidateDetailsPage = RunStep(
                "Save interview result",
                () => markInterviewPassedPage.SaveButtonClick());

            Log.Info("Verifying candidate status after the interview.");

            RunStep(
                "Verify candidate profile: Interview Passed",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Interview Passed"));

            Log.Info("Candidate successfully reached the Interview Passed stage.");

            // Offer the job.
            Log.Info("Opening the Offer Job form.");

            var offerJobPage = RunStep(
                "Open Offer Job form",
                () => candidateDetailsPage.ClickOfferJobButton());

            Log.Info("Saving the job offer.");

            candidateDetailsPage = RunStep(
                "Save job offer",
                () => offerJobPage.SaveButtonClick());

            Log.Info("Verifying candidate status after the job offer.");

            RunStep(
                "Verify candidate profile: Job Offered",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Job Offered"));

            Log.Info("Candidate successfully reached the Job Offered stage.");

            // Hire the candidate.
            Log.Info("Opening the Hire Candidate form.");

            var hireCandidatePage = RunStep(
                "Open Hire Candidate form",
                () => candidateDetailsPage.ClickHireButton());

            Log.Info("Saving the hiring decision.");

            candidateDetailsPage = RunStep(
                "Save hiring decision",
                () => hireCandidatePage.SaveButtonClick());

            Log.Info("Verifying candidate status after hiring.");

            RunStep(
                "Verify candidate profile: Hired",
                () => AssertCandidateDetails(
                    candidateDetailsPage,
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail,
                    vacancyName,
                    "Hired"));

            Log.Info(
                "Test passed: candidate completed the recruitment workflow and was hired.");
        }
        finally
        {
            // Clean up the candidate.
            Log.Info("Starting recruitment workflow test-data cleanup.");

            RunStep("Clean up candidate test data", () =>
            {
                TryDeleteCandidate(
                    candidatesApi,
                    candidateFirstName,
                    candidateLastName);
            });

            // Clean up the vacancy.
            Log.Info("Cleaning up the test vacancy.");

            RunStep("Clean up vacancy test data", () =>
            {
                TestDataCleanup.TryDelete(
                    "vacancy",
                    vacancyId,
                    vacanciesApi.DeleteVacancy);
            });

            // Clean up the hiring manager.
            Log.Info("Cleaning up the test hiring manager.");

            RunStep("Clean up hiring manager test data", () =>
            {
                TestDataCleanup.TryDelete(
                    "employee",
                    employeeId,
                    employeesApi.DeleteEmployee);
            });

            Log.Info("Recruitment workflow test-data cleanup completed.");
        }
    }

    private static void AssertCandidateDetails(
        CandidateDetailsPage candidateDetailsPage,
        string expectedFirstName,
        string expectedMiddleName,
        string expectedLastName,
        string expectedEmail,
        string expectedVacancy,
        string expectedStatus)
    {
        Log.Info(
            $"Verifying candidate profile. Expected status: {expectedStatus}.");

        RunStep("Verify candidate profile is displayed", () =>
        {
            Assert.That(
                candidateDetailsPage.IsDisplayed(),
                Is.True,
                "Candidate Profile page was not displayed.");
        });

        RunStep("Verify candidate profile fields", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    candidateDetailsPage.GetFirstName(),
                    Is.EqualTo(expectedFirstName),
                    "Candidate first name does not match.");

                Assert.That(
                    candidateDetailsPage.GetMiddleName(),
                    Is.EqualTo(expectedMiddleName),
                    "Candidate middle name does not match.");

                Assert.That(
                    candidateDetailsPage.GetLastName(),
                    Is.EqualTo(expectedLastName),
                    "Candidate last name does not match.");

                Assert.That(
                    candidateDetailsPage.GetEmail(),
                    Is.EqualTo(expectedEmail),
                    "Candidate email does not match.");

                Assert.That(
                    candidateDetailsPage.GetJobVacancy(),
                    Is.EqualTo(expectedVacancy),
                    "Candidate is linked to the wrong vacancy.");

                Assert.That(
                    candidateDetailsPage.IsResumeDisplayed(CvFileName),
                    Is.True,
                    "Uploaded CV is not displayed on the candidate profile.");

                Assert.That(
                    candidateDetailsPage.GetCandidateStatus(),
                    Is.EqualTo(expectedStatus),
                    $"Expected candidate status '{expectedStatus}'.");
            });
        });

        Log.Info(
            $"Candidate profile verified successfully. Expected status: {expectedStatus}.");
    }

    private static void TryDeleteCandidate(
        CandidatesApiClient candidatesApi,
        string firstName,
        string lastName)
    {
        Log.Info("Starting candidate cleanup.");

        try
        {
            Log.Info("Searching for the candidate through the API.");

            var response = RunStep(
                "Find candidate for cleanup through API",
                () => candidatesApi.GetCandidates(candidateName: firstName));

            if (!response.IsSuccessful ||
                string.IsNullOrWhiteSpace(response.Content))
            {
                Log.Warn(
                    $"Candidate cleanup skipped: GET request failed. " +
                    $"HTTP status: {(int)response.StatusCode}.");

                return;
            }

            var result = RunStep(
                "Deserialize candidate cleanup search response",
                () => JsonSerializer.Deserialize<CandidatesResponse>(
                    response.Content));

            var matchingCandidates = result?.Data
                .Where(candidate =>
                    candidate.FirstName == firstName &&
                    candidate.LastName == lastName)
                .ToList();

            if (matchingCandidates is null ||
                matchingCandidates.Count == 0)
            {
                Log.Warn(
                    "Candidate cleanup skipped: no matching candidate was found.");

                return;
            }

            Log.Info(
                $"Found {matchingCandidates.Count} matching candidate(s) to delete.");

            foreach (var candidate in matchingCandidates)
            {
                Log.Info($"Deleting candidate with ID: {candidate.Id}.");

                var deleteResponse = RunStep(
                    $"Delete candidate ID {candidate.Id} through API",
                    () => candidatesApi.DeleteCandidate(candidate.Id));

                if (!deleteResponse.IsSuccessful)
                {
                    Log.Warn(
                        $"Could not delete candidate with ID {candidate.Id}. " +
                        $"HTTP status: {(int)deleteResponse.StatusCode}.");

                    continue;
                }

                Log.Info($"Candidate with ID {candidate.Id} deleted successfully.");
            }

            Log.Info("Candidate cleanup completed.");
        }
        catch (Exception exception)
        {
            Log.Error(
                "Candidate cleanup failed with an unexpected exception.",
                exception);
        }
    }
}