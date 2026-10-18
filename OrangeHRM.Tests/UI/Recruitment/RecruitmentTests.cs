using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Recruitment;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.TestData;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.UI.Recruitment;

[TestFixture]
public class RecruitmentTests : UiTestBase
{
    [SetUp]
    public void SetUp()
    {
        var loginPage = new LoginPage();
        loginPage.Login(TestSettings.Username, TestSettings.Password);
    }

    [Test]
    public void CreateCandidate()
    {
        var firstName = CandidateTestData.FirstName;
        var lastName = CandidateTestData.LastName;
        var email = CandidateTestData.Email;
        var middleName = "Ivanov";
        var cvFileName = "candidate-cv.pdf";

        var cvFilePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "TestData",
            cvFileName);

        Assert.That(
            File.Exists(cvFilePath),
            Is.True,
            $"CV file was not found: {cvFilePath}");

        var testDataSetup = new TestDataSetup();
        var vacancyName = testDataSetup.CreateVacancyForHiringManager();

        DriverManager.Driver.Navigate().GoToUrl(
            new Uri(
                new Uri(TestSettings.BaseUrl),
                "web/index.php/dashboard/index").ToString());

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        var candidatesPage = recruitmentPage.OpenCandidates();
        var addCandidatePage = candidatesPage.ClickAdd();

        addCandidatePage.FillCandidateForm(
            firstName,
            middleName,
            lastName,
            email);

        addCandidatePage.SelectVacancy(vacancyName);
        addCandidatePage.UploadCv(cvFilePath);

        Assert.That(
            addCandidatePage.IsCvSelected(cvFileName),
            Is.True,
            "CV was not selected in the form.");

        var candidateDetailsPage = addCandidatePage.Save();

        Assert.That(candidateDetailsPage.GetFirstName(), Is.EqualTo(firstName));
        Assert.That(candidateDetailsPage.GetMiddleName(), Is.EqualTo(middleName));
        Assert.That(candidateDetailsPage.GetLastName(), Is.EqualTo(lastName));
        Assert.That(candidateDetailsPage.GetEmail(), Is.EqualTo(email));

        Assert.That(candidateDetailsPage.IsResumeDisplayed(cvFileName), Is.True,
            "Uploaded CV was not displayed on the candidate details page.");
        Assert.That(candidateDetailsPage.GetJobVacancy(), Is.EqualTo(vacancyName),
            "Selected job vacancy does not match.");
    }

    [Test]
    public void DeleteCreatedCandidate()
    {
        var firstName = "Candidate" + DateTime.Now.ToString("HHmmssfff");
        var middleName = "Ivanov";
        var lastName = "Automation";
        var email = $"candidate{DateTime.Now:HHmmssfff}@example.com";

        var cvFileName = "candidate-cv.pdf";
        var cvFilePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "TestData", cvFileName);

        Assert.That(File.Exists(cvFilePath), Is.True,
            $"CV file was not found: {cvFilePath}");
        var testDataSetup = new TestDataSetup();
        var vacancyName = testDataSetup.CreateVacancyForHiringManager();

        DriverManager.Driver.Navigate().GoToUrl(new Uri(new Uri(TestSettings.BaseUrl),
            "web/index.php/dashboard/index").ToString());
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        var candidatesPage = recruitmentPage.OpenCandidates();
        var addCandidatePage = candidatesPage.ClickAdd();

        addCandidatePage.FillCandidateForm(
            firstName,
            middleName,
            lastName,
            email);

        addCandidatePage.SelectVacancy(vacancyName);
        addCandidatePage.UploadCv(cvFilePath);

        Assert.That(addCandidatePage.IsCvSelected(cvFileName), Is.True,
            "CV was not selected in the form.");

        var candidateDetailsPage = addCandidatePage.Save();
        Assert.Multiple(() =>
        {
            Assert.That(candidateDetailsPage.GetFirstName(), Is.EqualTo(firstName));
            Assert.That(candidateDetailsPage.GetMiddleName(), Is.EqualTo(middleName));
            Assert.That(candidateDetailsPage.GetLastName(), Is.EqualTo(lastName));
            Assert.That(candidateDetailsPage.GetEmail(), Is.EqualTo(email));
            Assert.That(candidateDetailsPage.IsResumeDisplayed(cvFileName), Is.True,
                "Uploaded CV was not displayed on the candidate details page.");
            Assert.That(candidateDetailsPage.GetJobVacancy(), Is.EqualTo(vacancyName),
                "Selected job vacancy does not match.");
        });
        dashboardPage.SidePanel.OpenRecruitment();
        recruitmentPage = new RecruitmentPage();
        candidatesPage = recruitmentPage.OpenCandidates();
        candidatesPage.SearchCandidate(firstName);

        Assert.That(candidatesPage.IsCandidateDisplayed(firstName), Is.True,
            $"Candidate '{firstName}' was not found after filtering.");

        candidatesPage.DeleteCandidate(firstName);

        Assert.Multiple(() =>
        {
            Assert.That(candidatesPage.IsSuccessToastDisplayed(), Is.True,
                "Success alert was not displayed.");
            Assert.That(candidatesPage.GetSuccessToastTitle(), Is.EqualTo("Success"),
                "Unexpected alert title.");
            Assert.That(candidatesPage.GetSuccessToastMessage(), Is.EqualTo("Successfully Deleted"),
                "Unexpected alert message.");
        });

        candidatesPage.SearchCandidate(firstName);

        Assert.That(candidatesPage.IsCandidateDisplayed(firstName), Is.False,
            $"Candidate '{firstName}' is still displayed after deletion.");
    }

    [Test]
    public void CreateVacancy()
    {
        var testDataSetup = new TestDataSetup();
        var hiringManager = testDataSetup.CreateEmployeeForHiringManager();
        var vacancyName = $"QA Automation Vacancy {DateTime.Now:yyyyMMddHHmmssfff}";
        var jobTitle = "Software Engineer";
        DriverManager.Driver.Navigate().GoToUrl(
            new Uri(new Uri(TestSettings.BaseUrl),
                "web/index.php/dashboard/index").ToString());

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();
        var recruitmentPage = new RecruitmentPage();
        var vacanciesPage = recruitmentPage.OpenVacancies();
        var addVacancyPage = vacanciesPage.ClickAdd();

        addVacancyPage.EnterVacancyName(vacancyName);
        addVacancyPage.SelectJobTitle(jobTitle);
        addVacancyPage.SelectHiringManager(hiringManager);
        addVacancyPage.Save();

        var editVacancyPage = new EditVacancyPage();

        Assert.Multiple(() =>
        {
            Assert.That(editVacancyPage.GetVacancyName(), Is.EqualTo(vacancyName),
                "Vacancy name does not match.");
            Assert.That(editVacancyPage.GetJobTitle(), Is.EqualTo(jobTitle),
                "Job title does not match.");
            Assert.That(editVacancyPage.GetHiringManager(), Is.EqualTo(hiringManager),
                "Hiring manager does not match.");
        });
    }
}