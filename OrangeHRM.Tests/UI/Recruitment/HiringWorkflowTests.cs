using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;
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

    [SetUp]
    public void Login()
    {
        var dashboardPage = new LoginPage().Login(TestSettings.Username, TestSettings.Password);

        Assert.That(dashboardPage.IsDisplayed(), Is.True,
            "Dashboard page was not displayed after login.");
    }

    [Test]
    public void RecruitmentWorkflow_CandidateIsHiredSuccessfully()
    {
        var runId = Guid.NewGuid().ToString("N")[..10];
        
        var employeeFirstName = $"Manager{runId}";
        var employeeLastName = "E2E";
        var employeeName = $"{employeeFirstName} {employeeLastName}";
        var vacancyName = $"E2E Vacancy {runId}";

        var candidateFirstName = $"Candidate{runId}";
        var candidateMiddleName = "E2E";
        var candidateLastName = "Automation";
        var candidateEmail = $"candidate.{runId}@example.com";

        var cvFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", CvFileName);

        Assert.That(File.Exists(cvFilePath), Is.True,
            $"Candidate CV was not found: {cvFilePath}");

        var accessToken = Environment.GetEnvironmentVariable("ORANGEHRM_ACCESS_TOKEN");

        Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");

        using var employeesApi = new EmployeesApiClient(accessToken!);
        using var vacanciesApi = new VacanciesApiClient(accessToken!);
        using var candidatesApi = new CandidatesApiClient(accessToken!);

        int? employeeId = null;
        int? vacancyId = null;

        try
        {
            var employeeRequest = new CreateEmployeeRequest
            {
                FirstName = employeeFirstName,
                LastName = employeeLastName,
                MiddleName = string.Empty
            };

            var employeeResponse = employeesApi.CreateEmployee(employeeRequest);

            Assert.That(employeeResponse.IsSuccessful, Is.True,
                $"Employee creation failed. HTTP: " +
                $"{(int)employeeResponse.StatusCode}. " +
                $"Body: {employeeResponse.Content}");

            var createdEmployee = JsonSerializer.Deserialize<CreateEmployeeResponse>(employeeResponse.Content!);

            Assert.That(createdEmployee?.Data, Is.Not.Null);

            employeeId = createdEmployee!.Data!.EmpNumber;

            Assert.Multiple(() =>
            {
                Assert.That(employeeId.Value, Is.GreaterThan(0));

                Assert.That(createdEmployee.Data.FirstName, Is.EqualTo(employeeFirstName));

                Assert.That(createdEmployee.Data.LastName, Is.EqualTo(employeeLastName));
            });

            var vacancyRequest = new CreateVacancyRequest
            {
                Name = vacancyName,
                Status = true,
                JobTitleId = JobTitleId,
                IsPublished = true,
                Description = "Vacancy created for the E2E recruitment workflow.",
                NumOfPositions = 1,
                EmployeeId = employeeId.Value
            };

            var vacancyResponse = vacanciesApi.CreateVacancy(vacancyRequest);

            Assert.That(vacancyResponse.IsSuccessful, Is.True,
                $"Vacancy creation failed. HTTP: " +
                $"{(int)vacancyResponse.StatusCode}. " +
                $"Body: {vacancyResponse.Content}");

            var createdVacancy = JsonSerializer.Deserialize<CreateVacancyResponse>(vacancyResponse.Content!);

            Assert.That(createdVacancy?.Data, Is.Not.Null);

            var vacancyData = createdVacancy!.Data!;
            vacancyId = vacancyData.Id;

            Assert.Multiple(() =>
            {
                Assert.That(vacancyId.Value, Is.GreaterThan(0));
                Assert.That(vacancyData.Name, Is.EqualTo(vacancyName));
                Assert.That(vacancyData.JobTitle?.Id, Is.EqualTo(JobTitleId));
                Assert.That(vacancyData.JobTitle?.Title, Is.EqualTo(JobTitle));
                Assert.That(vacancyData.HiringManager?.Id, Is.EqualTo(employeeId.Value),
                    "Vacancy is linked to the wrong Hiring Manager.");
                Assert.That(vacancyData.HiringManager?.FirstName, Is.EqualTo(employeeFirstName));
                Assert.That(vacancyData.HiringManager?.LastName, Is.EqualTo(employeeLastName));
            });

            new DashboardPage().SidePanel.OpenRecruitment();

            var recruitmentPage = new RecruitmentPage();

            Assert.That(recruitmentPage.IsDisplayed(), Is.True,
                "Recruitment page was not displayed.");

            var candidatesPage = recruitmentPage.OpenCandidates();
            var addCandidatePage = candidatesPage.ClickAdd();

            addCandidatePage.FillCandidateForm(
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail);

            addCandidatePage.SelectVacancy(vacancyName);
            addCandidatePage.UploadCv(cvFilePath);

            Assert.That(addCandidatePage.IsCvSelected(CvFileName), Is.True,
                "CV was not selected in the Add Candidate form.");

            var candidateDetailsPage = addCandidatePage.Save();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Application Initiated");

            var shortlistPage = candidateDetailsPage.ShortlistButtonClick();

            Assert.That(shortlistPage.IsDisplayed(), Is.True,
                "Shortlist Candidate page was not displayed.");

            candidateDetailsPage = shortlistPage.SaveButtonClick();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Shortlisted");

            var scheduleInterviewPage = candidateDetailsPage.ClickScheduleInterviewButton();

            Assert.That(scheduleInterviewPage.IsDisplayed(), Is.True,
                "Schedule Interview page was not displayed.");

            scheduleInterviewPage.SelectInterviewer(employeeName);

            Assert.That(scheduleInterviewPage.GetInterviewer(), Is.EqualTo(employeeName),
                "The selected interviewer does not match the created employee.");

            scheduleInterviewPage.SelectDate(DateTime.Today.AddDays(1));
            scheduleInterviewPage.EnterInterviewTitle($"Technical Interview {runId}");
            candidateDetailsPage = scheduleInterviewPage.SaveButtonClick();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Interview Scheduled");

            var markInterviewPassedPage = candidateDetailsPage.ClickMarkInterviewPassedButton();

            candidateDetailsPage = markInterviewPassedPage.SaveButtonClick();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Interview Passed");

            var offerJobPage = candidateDetailsPage.ClickOfferJobButton();
            candidateDetailsPage = offerJobPage.SaveButtonClick();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Job Offered");

            var hireCandidatePage = candidateDetailsPage.ClickHireButton();
            candidateDetailsPage = hireCandidatePage.SaveButtonClick();

            AssertCandidateDetails(
                candidateDetailsPage,
                candidateFirstName,
                candidateMiddleName,
                candidateLastName,
                candidateEmail,
                vacancyName,
                "Hired");
        }
        finally
        {
            TryDeleteCandidate(
                candidatesApi,
                candidateFirstName,
                candidateLastName);

            TestDataCleanup.TryDelete(
                "vacancy",
                vacancyId,
                vacanciesApi.DeleteVacancy);

            TestDataCleanup.TryDelete(
                "employee",
                employeeId,
                employeesApi.DeleteEmployee);
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
        Assert.That(candidateDetailsPage.IsDisplayed(), Is.True,
            "Candidate Profile page was not displayed.");

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
    }

    private static void TryDeleteCandidate(
        CandidatesApiClient candidatesApi,
        string firstName,
        string lastName)
    {
        try
        {
            var response = candidatesApi.GetCandidates(
                candidateName: firstName);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                Console.WriteLine(
                    $"Candidate cleanup skipped: GET failed. " +
                    $"HTTP {(int)response.StatusCode}. {response.Content}");

                return;
            }

            var result =
                JsonSerializer.Deserialize<CandidatesResponse>(
                    response.Content);

            var matchingCandidates = result?.Data
                .Where(candidate =>
                    candidate.FirstName == firstName &&
                    candidate.LastName == lastName)
                .ToList();

            if (matchingCandidates is null || matchingCandidates.Count == 0)
            {
                Console.WriteLine(
                    "Candidate cleanup skipped: no matching candidate was found.");

                return;
            }

            foreach (var candidate in matchingCandidates)
            {
                var deleteResponse =
                    candidatesApi.DeleteCandidate(candidate.Id);

                if (!deleteResponse.IsSuccessful)
                {
                    Console.WriteLine(
                        $"WARNING: Could not delete candidate {candidate.Id}. " +
                        $"HTTP {(int)deleteResponse.StatusCode}. " +
                        $"{deleteResponse.Content}");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"WARNING: Candidate cleanup failed: {exception.Message}");
        }
    }
}