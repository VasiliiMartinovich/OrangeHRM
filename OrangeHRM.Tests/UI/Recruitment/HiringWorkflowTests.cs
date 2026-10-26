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
    private const string CvFileName = "candidate-cv.pdf";

    private static readonly ILog Log = LogManager.GetLogger(typeof(RecruitmentWorkflowTests));

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
            Assert.That(dashboardPage.IsDisplayed(), Is.True,
                "Dashboard page was not displayed after login.");
        });

        Log.Info("Recruitment workflow setup: login completed successfully.");
    }

    [Test]
    [AllureName("Recruitment Workflow - candidate is hired successfully")]
    [AllureTag("UI", "API", "EndToEnd", "HiringWorkflow")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Executes the end-to-end recruitment workflow. Creates a hiring manager " +
        "and job title through the API, creates a vacancy through the API, " +
        "creates a candidate through the UI with an uploaded CV, moves the candidate " +
        "through Shortlisted, Interview Scheduled, Interview Passed, and Job Offered, " +
        "and verifies the final Hired status. Attempts to clean up test data.")]
    public void RecruitmentWorkflow_CandidateIsHiredSuccessfully()
    {
        Log.Info("Test started: RecruitmentWorkflow_CandidateIsHiredSuccessfully.");

        var runId = Guid.NewGuid().ToString("N")[..10];

        var employeeFirstName = $"Manager{runId}";
        var employeeLastName = "E2E";
        var employeeName = $"{employeeFirstName} {employeeLastName}";

        var jobTitle = $"E2E Job {runId}";
        var vacancyName = $"E2E Vacancy {runId}";

        var candidateFirstName = $"Candidate{runId}";
        var candidateMiddleName = "E2E";
        var candidateLastName = "Automation";
        var candidateEmail = $"candidate.{runId}@example.com";

        Log.Info($"Generated unique test data. Run ID: {runId}.");
        Log.Info($"Candidate: {candidateFirstName} {candidateLastName}.");
        Log.Info($"Hiring manager: {employeeName}.");
        Log.Info($"Job title: {jobTitle}.");
        Log.Info($"Vacancy: {vacancyName}.");

        var cvFilePath = RunStep(
            "Resolve candidate CV file path",
            () => Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "TestData",
                CvFileName));

        RunStep("Verify candidate CV file exists", () =>
        {
            Assert.That(File.Exists(cvFilePath), Is.True,
                $"Candidate CV was not found: {cvFilePath}");
        });

        Log.Info("Reading the OrangeHRM API access token from the environment.");

        var accessToken = RunStep(
            "Read API access token from environment",
            () => Environment.GetEnvironmentVariable("ORANGEHRM_ACCESS_TOKEN"));

        RunStep("Verify API access token is configured", () =>
        {
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty,
                "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");
        });

        using var employeesApi = new EmployeesApiClient(accessToken!);
        using var jobTitlesApi = new JobTitlesApiClient(accessToken!);
        using var vacanciesApi = new VacanciesApiClient(accessToken!);
        using var candidatesApi = new CandidatesApiClient(accessToken!);

        int? employeeId = null;
        int? jobTitleId = null;
        int? vacancyId = null;

        try
        {
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

            RunStep("Verify hiring manager creation response", () =>
            {
                Assert.That(employeeResponse.IsSuccessful, Is.True,
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
                    Assert.That(createdEmployee.Data.FirstName, Is.EqualTo(employeeFirstName));
                    Assert.That(createdEmployee.Data.LastName, Is.EqualTo(employeeLastName));
                });
            });

            Log.Info($"Hiring manager created and verified. Employee ID: {employeeId.Value}.");

            Log.Info($"Creating job title through the API: {jobTitle}.");

            var jobTitleRequest = RunStep(
                "Prepare job title request",
                () => new CreateJobTitleRequest
                {
                    Title = jobTitle,
                    Description = "Job title created for the E2E recruitment workflow.",
                    Note = "Created by automated test."
                });

            var jobTitleResponse = RunStep(
                "Create job title through API",
                () => jobTitlesApi.CreateJobTitle(jobTitleRequest));

            RunStep("Verify job title creation response", () =>
            {
                Assert.That(jobTitleResponse.IsSuccessful, Is.True,
                    $"Job title creation failed. HTTP: " +
                    $"{(int)jobTitleResponse.StatusCode}. " +
                    $"Body: {jobTitleResponse.Content}");
            });

            var createdJobTitle = RunStep(
                "Deserialize created job title response",
                () => JsonSerializer.Deserialize<CreateJobTitleResponse>(
                    jobTitleResponse.Content!));

            RunStep("Verify created job title data", () =>
            {
                Assert.That(createdJobTitle?.Data, Is.Not.Null);
            });

            jobTitleId = createdJobTitle!.Data!.Id;

            RunStep("Verify created job title fields", () =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(jobTitleId.Value, Is.GreaterThan(0));
                    Assert.That(createdJobTitle.Data.Title, Is.EqualTo(jobTitle));
                });
            });

            Log.Info($"Job title created and verified. Job title ID: {jobTitleId.Value}.");
            Log.Info($"Creating vacancy through the API: {vacancyName}.");

            var vacancyRequest = RunStep(
                "Prepare vacancy request",
                () => new CreateVacancyRequest
                {
                    Name = vacancyName,
                    Status = true,
                    JobTitleId = jobTitleId.Value,
                    IsPublished = true,
                    Description = "Vacancy created for the E2E recruitment workflow.",
                    NumOfPositions = 1,
                    EmployeeId = employeeId.Value
                });

            var vacancyResponse = RunStep(
                "Create vacancy through API",
                () => vacanciesApi.CreateVacancy(vacancyRequest));

            RunStep("Verify vacancy creation response", () =>
            {
                Assert.That(vacancyResponse.IsSuccessful, Is.True,
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
                    Assert.That(vacancyData.Name, Is.EqualTo(vacancyName));
                    Assert.That(vacancyData.JobTitle?.Id, Is.EqualTo(jobTitleId.Value));
                    Assert.That(vacancyData.JobTitle?.Title, Is.EqualTo(jobTitle));
                    Assert.That(vacancyData.HiringManager?.Id, Is.EqualTo(employeeId.Value),
                        "Vacancy is linked to the wrong Hiring Manager.");
                    Assert.That(vacancyData.HiringManager?.FirstName, Is.EqualTo(employeeFirstName));
                    Assert.That(vacancyData.HiringManager?.LastName, Is.EqualTo(employeeLastName));
                });
            });

            Log.Info($"Vacancy created and verified. Vacancy ID: {vacancyId.Value}.");

            RunStep("Navigate to Dashboard", () =>
            {
                DriverManager.Driver.Navigate().GoToUrl(new Uri(new Uri(TestSettings.BaseUrl), "web/index.php/dashboard/index").ToString());
            });

            var dashboardPage = RunStep(
                "Create DashboardPage object",
                () => new DashboardPage());

            RunStep(
                "Open Recruitment section",
                () => dashboardPage.SidePanel.OpenRecruitment());

            var recruitmentPage = RunStep(
                "Create RecruitmentPage object",
                () => new RecruitmentPage());

            RunStep("Verify Recruitment page is displayed", () =>
            {
                Assert.That(recruitmentPage.IsDisplayed(), Is.True,
                    "Recruitment page was not displayed.");
            });

            var candidatesPage = RunStep(
                "Open candidates list",
                () => recruitmentPage.OpenCandidates());

            var addCandidatePage = RunStep(
                "Open Add Candidate form",
                () => candidatesPage.ClickAdd());

            RunStep("Fill in candidate information", () =>
            {
                addCandidatePage.FillCandidateForm(
                    candidateFirstName,
                    candidateMiddleName,
                    candidateLastName,
                    candidateEmail);
            });

            RunStep(
                "Select vacancy for candidate",
                () => addCandidatePage.SelectVacancy(vacancyName));

            RunStep(
                "Upload candidate CV",
                () => addCandidatePage.UploadCv(cvFilePath));

            RunStep("Verify CV is selected", () =>
            {
                Assert.That(
                    addCandidatePage.IsCvSelected(CvFileName),
                    Is.True,
                    "CV was not selected in the Add Candidate form.");
            });

            var candidateDetailsPage = RunStep(
                "Save candidate",
                () => addCandidatePage.Save());

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

            var shortlistPage = RunStep(
                "Open Shortlist Candidate form",
                () => candidateDetailsPage.ShortlistButtonClick());

            RunStep("Verify Shortlist Candidate page is displayed", () =>
            {
                Assert.That(shortlistPage.IsDisplayed(), Is.True,
                    "Shortlist Candidate page was not displayed.");
            });

            candidateDetailsPage = RunStep(
                "Save Shortlisted stage",
                () => shortlistPage.SaveButtonClick());

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

            var scheduleInterviewPage = RunStep(
                "Open Schedule Interview form",
                () => candidateDetailsPage.ClickScheduleInterviewButton());

            RunStep("Verify Schedule Interview page is displayed", () =>
            {
                Assert.That(scheduleInterviewPage.IsDisplayed(), Is.True,
                    "Schedule Interview page was not displayed.");
            });

            RunStep(
                "Select hiring manager as interviewer",
                () => scheduleInterviewPage.SelectInterviewer(employeeName));

            RunStep("Verify selected interviewer", () =>
            {
                Assert.That(scheduleInterviewPage.GetInterviewer(), Is.EqualTo(employeeName),
                    "The selected interviewer does not match the created employee.");
            });

            var interviewDate = DateTime.Today.AddDays(1);
            var interviewTitle = $"Technical Interview {runId}";

            RunStep(
                "Select interview date",
                () => scheduleInterviewPage.SelectDate(interviewDate));

            RunStep(
                "Enter interview title",
                () => scheduleInterviewPage.EnterInterviewTitle(interviewTitle));

            candidateDetailsPage = RunStep(
                "Save scheduled interview",
                () => scheduleInterviewPage.SaveButtonClick());

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

            var markInterviewPassedPage = RunStep(
                "Open Mark Interview Passed form",
                () => candidateDetailsPage.ClickMarkInterviewPassedButton());

            candidateDetailsPage = RunStep(
                "Save interview result",
                () => markInterviewPassedPage.SaveButtonClick());

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

            var offerJobPage = RunStep(
                "Open Offer Job form",
                () => candidateDetailsPage.ClickOfferJobButton());

            candidateDetailsPage = RunStep(
                "Save job offer",
                () => offerJobPage.SaveButtonClick());

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

            var hireCandidatePage = RunStep(
                "Open Hire Candidate form",
                () => candidateDetailsPage.ClickHireButton());

            candidateDetailsPage = RunStep(
                "Save hiring decision",
                () => hireCandidatePage.SaveButtonClick());

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

            Log.Info("Test passed: candidate completed the recruitment workflow and was hired.");
        }
        finally
        {
            Log.Info("Starting recruitment workflow test-data cleanup.");

            RunStep("Clean up candidate test data", () =>
            {
                TryDeleteCandidate(
                    candidatesApi,
                    candidateFirstName,
                    candidateLastName);
            });

            RunStep("Clean up vacancy test data", () =>
            {
                TestDataCleanup.TryDelete(
                    "vacancy",
                    vacancyId,
                    vacanciesApi.DeleteVacancy);
            });

            RunStep("Clean up job title test data", () =>
            {
                TestDataCleanup.TryDelete(
                    "job title",
                    jobTitleId,
                    jobTitlesApi.DeleteJobTitle);
            });

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
        Log.Info($"Verifying candidate profile. Expected status: {expectedStatus}.");

        RunStep("Verify candidate profile is displayed", () =>
        {
            Assert.That(candidateDetailsPage.IsDisplayed(), Is.True,
                "Candidate Profile page was not displayed.");
        });

        RunStep("Verify candidate profile fields", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(candidateDetailsPage.GetFirstName(), Is.EqualTo(expectedFirstName),
                    "Candidate first name does not match.");

                Assert.That(candidateDetailsPage.GetMiddleName(), Is.EqualTo(expectedMiddleName),
                    "Candidate middle name does not match.");
                Assert.That(candidateDetailsPage.GetLastName(), Is.EqualTo(expectedLastName),
                    "Candidate last name does not match.");
                Assert.That(candidateDetailsPage.GetEmail(), Is.EqualTo(expectedEmail),
                    "Candidate email does not match.");
                Assert.That(candidateDetailsPage.GetJobVacancy(), Is.EqualTo(expectedVacancy),
                    "Candidate is linked to the wrong vacancy.");
                Assert.That(candidateDetailsPage.IsResumeDisplayed(CvFileName), Is.True,
                    "Uploaded CV is not displayed on the candidate profile.");
                Assert.That(candidateDetailsPage.GetCandidateStatus(), Is.EqualTo(expectedStatus),
                    $"Expected candidate status '{expectedStatus}'.");
            });
        });

        Log.Info($"Candidate profile verified successfully. Expected status: {expectedStatus}.");
    }

    private static void TryDeleteCandidate(
        CandidatesApiClient candidatesApi,
        string firstName,
        string lastName)
    {
        Log.Info("Starting candidate cleanup.");

        try
        {
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

            Log.Info($"Found {matchingCandidates.Count} matching candidate(s) to delete.");

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
            Log.Error("Candidate cleanup failed with an unexpected exception.", exception);
        }
    }
}