using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using log4net;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Recruitment;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.UI.Recruitment;

[TestFixture]
public class RecruitmentTests : UiTestBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(RecruitmentTests));

    [SetUp]
    public void SetUp()
    {
        Log.Info("RecruitmentTests setup: logging in to OrangeHRM.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        AllureApi.Step(
            "Log in with configured valid credentials",
            () => loginPage.Login(
                TestSettings.Username,
                TestSettings.Password));

        Log.Info("RecruitmentTests setup: login completed.");
    }

    [Test]
    [AllureName("Recruitment - create candidate with CV and vacancy")]
    [AllureTag("UI", "Recruitment", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that a candidate can be created with first name, middle name, last name, email, a selected vacancy, and an uploaded CV. The saved candidate details are checked against the submitted data.")]
    public void CreateCandidate()
    {
        Log.Info("Test started: CreateCandidate.");

        var candidateData = AllureApi.Step(
            "Prepare candidate test data",
            () => new
            {
                FirstName = CandidateTestData.FirstName,
                LastName = CandidateTestData.LastName,
                Email = CandidateTestData.Email,
                MiddleName = "Ivanov",
                CvFileName = "candidate-cv.pdf"
            });

        var firstName = candidateData.FirstName;
        var lastName = candidateData.LastName;
        var email = candidateData.Email;
        var middleName = candidateData.MiddleName;
        var cvFileName = candidateData.CvFileName;

        Log.Info(
            $"Preparing candidate data. First name: {firstName}, last name: {lastName}.");

        var cvFilePath = AllureApi.Step(
            "Resolve CV file path",
            () => Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "TestData",
                cvFileName));

        Log.Info($"Checking that the CV file exists: {cvFileName}.");

        AllureApi.Step("Verify CV file exists", () =>
        {
            Assert.That(
                File.Exists(cvFilePath),
                Is.True,
                $"CV file was not found: {cvFilePath}");
        });

        Log.Info("CV file exists.");

        var testDataSetup = new TestDataSetup();

        Log.Info("Creating a vacancy for the hiring manager.");

        var vacancyName = AllureApi.Step(
            "Create vacancy for candidate application",
            () => testDataSetup.CreateVacancyForHiringManager());

        Log.Info($"Vacancy created: {vacancyName}.");

        Log.Info("Navigating to the OrangeHRM dashboard.");

        AllureApi.Step("Navigate to Dashboard", () =>
        {
            DriverManager.Driver.Navigate().GoToUrl(
                new Uri(
                    new Uri(TestSettings.BaseUrl),
                    "web/index.php/dashboard/index").ToString());
        });

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening the Recruitment section.");

        AllureApi.Step(
            "Open Recruitment section",
            () => dashboardPage.SidePanel.OpenRecruitment());

        var recruitmentPage = AllureApi.Step(
            "Create RecruitmentPage object",
            () => new RecruitmentPage());

        Log.Info("Opening the candidates list.");

        var candidatesPage = AllureApi.Step(
            "Open candidates list",
            () => recruitmentPage.OpenCandidates());

        Log.Info("Opening the Add Candidate form.");

        var addCandidatePage = AllureApi.Step(
            "Open Add Candidate form",
            () => candidatesPage.ClickAdd());

        Log.Info("Filling in the candidate form.");

        AllureApi.Step("Fill in candidate information", () =>
        {
            addCandidatePage.FillCandidateForm(
                firstName,
                middleName,
                lastName,
                email);
        });

        Log.Info($"Selecting vacancy: {vacancyName}.");

        AllureApi.Step(
            "Select vacancy for candidate",
            () => addCandidatePage.SelectVacancy(vacancyName));

        Log.Info($"Uploading CV file: {cvFileName}.");

        AllureApi.Step(
            "Upload candidate CV",
            () => addCandidatePage.UploadCv(cvFilePath));

        Log.Info("Checking that the CV is selected in the form.");

        AllureApi.Step("Verify CV is selected", () =>
        {
            Assert.That(
                addCandidatePage.IsCvSelected(cvFileName),
                Is.True,
                "CV was not selected in the form.");
        });

        Log.Info("CV is selected. Saving the candidate.");

        var candidateDetailsPage = AllureApi.Step(
            "Save candidate",
            () => addCandidatePage.Save());

        Log.Info("Verifying the saved candidate details.");

        AllureApi.Step("Verify saved candidate information", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    candidateDetailsPage.GetFirstName(),
                    Is.EqualTo(firstName));

                Assert.That(
                    candidateDetailsPage.GetMiddleName(),
                    Is.EqualTo(middleName));

                Assert.That(
                    candidateDetailsPage.GetLastName(),
                    Is.EqualTo(lastName));

                Assert.That(
                    candidateDetailsPage.GetEmail(),
                    Is.EqualTo(email));
            });
        });

        Log.Info("Verifying that the uploaded CV is displayed.");

        AllureApi.Step("Verify uploaded CV is displayed", () =>
        {
            Assert.That(
                candidateDetailsPage.IsResumeDisplayed(cvFileName),
                Is.True,
                "Uploaded CV was not displayed on the candidate details page.");
        });

        Log.Info("Verifying that the selected vacancy matches.");

        AllureApi.Step("Verify selected vacancy", () =>
        {
            Assert.That(
                candidateDetailsPage.GetJobVacancy(),
                Is.EqualTo(vacancyName),
                "Selected job vacancy does not match.");
        });

        Log.Info("Test passed: candidate was created and all details were verified.");
    }

    [Test]
    [AllureName("Recruitment - delete created candidate")]
    [AllureTag("UI", "Recruitment", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates a candidate with an uploaded CV and a selected vacancy, verifies the candidate appears in search results, deletes the candidate, checks the success notification, and verifies the candidate is no longer displayed.")]
    public void DeleteCreatedCandidate()
    {
        Log.Info("Test started: DeleteCreatedCandidate.");

        var candidateData = AllureApi.Step(
            "Prepare candidate test data",
            () => new
            {
                FirstName = "Candidate" + DateTime.Now.ToString("HHmmssfff"),
                MiddleName = "Ivanov",
                LastName = "Automation",
                Email = $"candidate{DateTime.Now:HHmmssfff}@example.com",
                CvFileName = "candidate-cv.pdf"
            });

        var firstName = candidateData.FirstName;
        var middleName = candidateData.MiddleName;
        var lastName = candidateData.LastName;
        var email = candidateData.Email;
        var cvFileName = candidateData.CvFileName;

        Log.Info(
            $"Preparing candidate data. First name: {firstName}, last name: {lastName}.");

        var cvFilePath = AllureApi.Step(
            "Resolve CV file path",
            () => Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "TestData",
                cvFileName));

        Log.Info($"Checking that the CV file exists: {cvFileName}.");

        AllureApi.Step("Verify CV file exists", () =>
        {
            Assert.That(
                File.Exists(cvFilePath),
                Is.True,
                $"CV file was not found: {cvFilePath}");
        });

        Log.Info("CV file exists.");

        var testDataSetup = new TestDataSetup();

        Log.Info("Creating a vacancy for the hiring manager.");

        var vacancyName = AllureApi.Step(
            "Create vacancy for candidate application",
            () => testDataSetup.CreateVacancyForHiringManager());

        Log.Info($"Vacancy created: {vacancyName}.");

        Log.Info("Navigating to the OrangeHRM dashboard.");

        AllureApi.Step("Navigate to Dashboard", () =>
        {
            DriverManager.Driver.Navigate().GoToUrl(
                new Uri(
                    new Uri(TestSettings.BaseUrl),
                    "web/index.php/dashboard/index").ToString());
        });

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening the Recruitment section.");

        AllureApi.Step(
            "Open Recruitment section",
            () => dashboardPage.SidePanel.OpenRecruitment());

        var recruitmentPage = AllureApi.Step(
            "Create RecruitmentPage object",
            () => new RecruitmentPage());

        Log.Info("Opening the candidates list.");

        var candidatesPage = AllureApi.Step(
            "Open candidates list",
            () => recruitmentPage.OpenCandidates());

        Log.Info("Opening the Add Candidate form.");

        var addCandidatePage = AllureApi.Step(
            "Open Add Candidate form",
            () => candidatesPage.ClickAdd());

        Log.Info("Filling in the candidate form.");

        AllureApi.Step("Fill in candidate information", () =>
        {
            addCandidatePage.FillCandidateForm(
                firstName,
                middleName,
                lastName,
                email);
        });

        Log.Info($"Selecting vacancy: {vacancyName}.");

        AllureApi.Step(
            "Select vacancy for candidate",
            () => addCandidatePage.SelectVacancy(vacancyName));

        Log.Info($"Uploading CV file: {cvFileName}.");

        AllureApi.Step(
            "Upload candidate CV",
            () => addCandidatePage.UploadCv(cvFilePath));

        Log.Info("Checking that the CV is selected in the form.");

        AllureApi.Step("Verify CV is selected", () =>
        {
            Assert.That(
                addCandidatePage.IsCvSelected(cvFileName),
                Is.True,
                "CV was not selected in the form.");
        });

        Log.Info("CV is selected. Saving the candidate.");

        var candidateDetailsPage = AllureApi.Step(
            "Save candidate",
            () => addCandidatePage.Save());

        Log.Info("Verifying the created candidate details.");

        AllureApi.Step("Verify saved candidate details", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(candidateDetailsPage.GetFirstName(), Is.EqualTo(firstName));
                Assert.That(candidateDetailsPage.GetMiddleName(), Is.EqualTo(middleName));
                Assert.That(candidateDetailsPage.GetLastName(), Is.EqualTo(lastName));
                Assert.That(candidateDetailsPage.GetEmail(), Is.EqualTo(email));

                Assert.That(
                    candidateDetailsPage.IsResumeDisplayed(cvFileName),
                    Is.True,
                    "Uploaded CV was not displayed on the candidate details page.");

                Assert.That(
                    candidateDetailsPage.GetJobVacancy(),
                    Is.EqualTo(vacancyName),
                    "Selected job vacancy does not match.");
            });
        });

        Log.Info("Candidate details verified. Returning to the candidates list.");

        AllureApi.Step(
            "Open Recruitment section",
            () => dashboardPage.SidePanel.OpenRecruitment());

        recruitmentPage = AllureApi.Step(
            "Create RecruitmentPage object",
            () => new RecruitmentPage());

        Log.Info("Opening the candidates list.");

        candidatesPage = AllureApi.Step(
            "Open candidates list",
            () => recruitmentPage.OpenCandidates());

        Log.Info($"Searching for the created candidate: {firstName}.");

        AllureApi.Step(
            "Search for created candidate",
            () => candidatesPage.SearchCandidate(firstName));

        Log.Info("Verifying that the candidate is displayed before deletion.");

        AllureApi.Step("Verify candidate appears before deletion", () =>
        {
            Assert.That(
                candidatesPage.IsCandidateDisplayed(firstName),
                Is.True,
                $"Candidate '{firstName}' was not found after filtering.");
        });

        Log.Info($"Deleting candidate: {firstName}.");

        AllureApi.Step(
            "Delete candidate",
            () => candidatesPage.DeleteCandidate(firstName));

        Log.Info("Verifying the deletion success notification.");

        AllureApi.Step("Verify deletion success notification", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    candidatesPage.IsSuccessToastDisplayed(),
                    Is.True,
                    "Success alert was not displayed.");

                Assert.That(
                    candidatesPage.GetSuccessToastTitle(),
                    Is.EqualTo("Success"),
                    "Unexpected alert title.");

                Assert.That(
                    candidatesPage.GetSuccessToastMessage(),
                    Is.EqualTo("Successfully Deleted"),
                    "Unexpected alert message.");
            });
        });

        Log.Info("Deletion success notification verified.");

        Log.Info($"Searching for candidate {firstName} after deletion.");

        AllureApi.Step(
            "Search for candidate after deletion",
            () => candidatesPage.SearchCandidate(firstName));

        Log.Info("Verifying that the deleted candidate is no longer displayed.");

        AllureApi.Step("Verify candidate is absent after deletion", () =>
        {
            Assert.That(
                candidatesPage.IsCandidateDisplayed(firstName),
                Is.False,
                $"Candidate '{firstName}' is still displayed after deletion.");
        });

        Log.Info("Test passed: candidate was created, found, deleted, and verified absent.");
    }

    [Test]
    [AllureName("Recruitment - create vacancy")]
    [AllureTag("UI", "Recruitment", "Vacancies")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates an employee to use as the hiring manager, creates a vacancy with the specified job title, and verifies the saved vacancy name, job title, and hiring manager.")]
    public void CreateVacancy()
    {
        Log.Info("Test started: CreateVacancy.");

        var testDataSetup = new TestDataSetup();

        Log.Info("Creating an employee to use as the hiring manager.");

        var hiringManager = AllureApi.Step(
            "Create employee for hiring manager",
            () => testDataSetup.CreateEmployeeForHiringManager());

        Log.Info($"Hiring manager created: {hiringManager}.");

        var vacancyName = $"QA Automation Vacancy {DateTime.Now:yyyyMMddHHmmssfff}";
        var jobTitle = "Software Engineer";

        Log.Info(
            $"Preparing vacancy data. Vacancy name: {vacancyName}, job title: {jobTitle}.");

        Log.Info("Navigating to the OrangeHRM dashboard.");

        AllureApi.Step("Navigate to Dashboard", () =>
        {
            DriverManager.Driver.Navigate().GoToUrl(
                new Uri(
                    new Uri(TestSettings.BaseUrl),
                    "web/index.php/dashboard/index").ToString());
        });

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening the Recruitment section.");

        AllureApi.Step(
            "Open Recruitment section",
            () => dashboardPage.SidePanel.OpenRecruitment());

        var recruitmentPage = AllureApi.Step(
            "Create RecruitmentPage object",
            () => new RecruitmentPage());

        Log.Info("Opening the vacancies list.");

        var vacanciesPage = AllureApi.Step(
            "Open vacancies list",
            () => recruitmentPage.OpenVacancies());

        Log.Info("Opening the Add Vacancy form.");

        var addVacancyPage = AllureApi.Step(
            "Open Add Vacancy form",
            () => vacanciesPage.ClickAdd());

        Log.Info($"Entering vacancy name: {vacancyName}.");

        AllureApi.Step(
            "Enter vacancy name",
            () => addVacancyPage.EnterVacancyName(vacancyName));

        Log.Info($"Selecting job title: {jobTitle}.");

        AllureApi.Step(
            "Select job title",
            () => addVacancyPage.SelectJobTitle(jobTitle));

        Log.Info($"Selecting hiring manager: {hiringManager}.");

        AllureApi.Step(
            "Select hiring manager",
            () => addVacancyPage.SelectHiringManager(hiringManager));

        Log.Info("Saving the vacancy.");

        AllureApi.Step(
            "Save vacancy",
            () => addVacancyPage.Save());

        var editVacancyPage = AllureApi.Step(
            "Create EditVacancyPage object",
            () => new EditVacancyPage());

        Log.Info("Verifying the saved vacancy details.");

        AllureApi.Step("Verify saved vacancy information", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    editVacancyPage.GetVacancyName(),
                    Is.EqualTo(vacancyName),
                    "Vacancy name does not match.");

                Assert.That(
                    editVacancyPage.GetJobTitle(),
                    Is.EqualTo(jobTitle),
                    "Job title does not match.");

                Assert.That(
                    editVacancyPage.GetHiringManager(),
                    Is.EqualTo(hiringManager),
                    "Hiring manager does not match.");
            });
        });

        Log.Info("Test passed: vacancy was created and all details were verified.");
    }
}