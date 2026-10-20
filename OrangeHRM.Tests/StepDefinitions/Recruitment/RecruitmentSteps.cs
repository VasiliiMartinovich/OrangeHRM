using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Recruitment;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Infrastructure;
using Reqnroll;

namespace OrangeHRM.Tests.StepDefinitions.Recruitment;

[Binding]
public class RecruitmentSteps
{
    private string _firstName = null!;
    private string _middleName = "Ivanov";
    private string _lastName = "Automation";
    private string _email = null!;
    private string _cvFileName = "candidate-cv.pdf";
    private string _cvFilePath = null!;
    private string _vacancyName = null!;
    private string _jobTitle = null!;
    private string _hiringManager = null!;

    private CandidateDetailsPage _candidateDetailsPage = null!;
    private CandidatesPage _candidatesPage = null!;
    private EditVacancyPage _editVacancyPage = null!;

    [Given("a vacancy is available for hiring")]
    public void GivenAVacancyIsAvailableForHiring()
    {
        _vacancyName = new TestDataSetup()
            .CreateVacancyForHiringManager();
    }

    [When("I create a candidate with a CV and apply for the vacancy")]
    public void WhenICreateCandidateWithCvAndApplyForVacancy()
    {
        _firstName = $"Candidate{DateTime.Now:HHmmssfff}";
        _middleName = "Ivanov";
        _lastName = "Automation";
        _email = $"candidate{DateTime.Now:HHmmssfff}@example.com";
        _cvFileName = "candidate-cv.pdf";

        _cvFilePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "TestData",
            _cvFileName);

        Assert.That(File.Exists(_cvFilePath), Is.True,
            $"CV file was not found: {_cvFilePath}");

        var dashboardUrl = new Uri(
            new Uri(TestSettings.BaseUrl),
            "web/index.php/dashboard/index").ToString();

        DriverManager.Driver.Navigate().GoToUrl(dashboardUrl);

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        var candidatesPage = recruitmentPage.OpenCandidates();
        var addCandidatePage = candidatesPage.ClickAdd();

        addCandidatePage.FillCandidateForm(
            _firstName,
            _middleName,
            _lastName,
            _email);

        addCandidatePage.SelectVacancy(_vacancyName);
        addCandidatePage.UploadCv(_cvFilePath);

        Assert.That(
            addCandidatePage.IsCvSelected(_cvFileName),
            Is.True,
            "CV was not selected in the form.");

        _candidateDetailsPage = addCandidatePage.Save();
    }

    [Then("candidate details should match the submitted information")]
    public void ThenCandidateDetailsShouldMatchSubmittedInformation()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                _candidateDetailsPage.GetFirstName(),
                Is.EqualTo(_firstName));

            Assert.That(
                _candidateDetailsPage.GetMiddleName(),
                Is.EqualTo(_middleName));

            Assert.That(
                _candidateDetailsPage.GetLastName(),
                Is.EqualTo(_lastName));

            Assert.That(
                _candidateDetailsPage.GetEmail(),
                Is.EqualTo(_email));
        });
    }

    [Then("the uploaded CV and selected vacancy should be displayed")]
    public void ThenUploadedCvAndSelectedVacancyShouldBeDisplayed()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                _candidateDetailsPage.IsResumeDisplayed(_cvFileName),
                Is.True,
                "Uploaded CV was not displayed on the candidate details page.");

            Assert.That(
                _candidateDetailsPage.GetJobVacancy(),
                Is.EqualTo(_vacancyName),
                "Selected vacancy does not match.");
        });
    }

    [Given("a candidate with a CV and vacancy exists")]
    public void GivenCandidateWithCvAndVacancyExists()
    {
        _vacancyName = new TestDataSetup()
            .CreateVacancyForHiringManager();

        _firstName = $"Candidate{DateTime.Now:HHmmssfff}";
        _middleName = "Ivanov";
        _lastName = "Automation";
        _email = $"candidate{DateTime.Now:HHmmssfff}@example.com";
        _cvFileName = "candidate-cv.pdf";

        _cvFilePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "TestData",
            _cvFileName);

        Assert.That(File.Exists(_cvFilePath), Is.True,
            $"CV file was not found: {_cvFilePath}");

        var dashboardUrl = new Uri(
            new Uri(TestSettings.BaseUrl),
            "web/index.php/dashboard/index").ToString();

        DriverManager.Driver.Navigate().GoToUrl(dashboardUrl);

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        var candidatesPage = recruitmentPage.OpenCandidates();
        var addCandidatePage = candidatesPage.ClickAdd();

        addCandidatePage.FillCandidateForm(
            _firstName,
            _middleName,
            _lastName,
            _email);

        addCandidatePage.SelectVacancy(_vacancyName);
        addCandidatePage.UploadCv(_cvFilePath);

        Assert.That(
            addCandidatePage.IsCvSelected(_cvFileName),
            Is.True,
            "CV was not selected in the form.");

        _candidateDetailsPage = addCandidatePage.Save();
    }

    [When("I delete the candidate from the candidates list")]
    public void WhenIDeleteCandidateFromCandidatesList()
    {
        var dashboardUrl = new Uri(
            new Uri(TestSettings.BaseUrl),
            "web/index.php/dashboard/index").ToString();

        DriverManager.Driver.Navigate().GoToUrl(dashboardUrl);

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        _candidatesPage = recruitmentPage.OpenCandidates();

        _candidatesPage.SearchCandidate(_firstName);

        Assert.That(
            _candidatesPage.IsCandidateDisplayed(_firstName),
            Is.True,
            $"Candidate '{_firstName}' was not found before deletion.");

        _candidatesPage.DeleteCandidate(_firstName);
    }

    [Then("the candidate deletion confirmation should be displayed")]
    public void ThenCandidateDeletionConfirmationShouldBeDisplayed()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                _candidatesPage.IsSuccessToastDisplayed(),
                Is.True,
                "Success notification was not displayed.");

            Assert.That(
                _candidatesPage.GetSuccessToastTitle(),
                Is.EqualTo("Success"));

            Assert.That(
                _candidatesPage.GetSuccessToastMessage(),
                Is.EqualTo("Successfully Deleted"));
        });
    }

    [Then("the deleted candidate should no longer appear in search results")]
    public void ThenDeletedCandidateShouldNotAppearInSearchResults()
    {
        _candidatesPage.SearchCandidate(_firstName);

        Assert.That(
            _candidatesPage.IsCandidateDisplayed(_firstName),
            Is.False,
            $"Candidate '{_firstName}' is still displayed after deletion.");
    }

    [Given("an employee is available as hiring manager")]
    public void GivenEmployeeIsAvailableAsHiringManager()
    {
        _hiringManager = new TestDataSetup()
            .CreateEmployeeForHiringManager();
    }

    [When("I create a vacancy for the job title {string}")]
    public void WhenICreateVacancyForJobTitle(string jobTitle)
    {
        _jobTitle = jobTitle;
        _vacancyName = $"QA Automation Vacancy {DateTime.Now:yyyyMMddHHmmssfff}";

        var dashboardUrl = new Uri(
            new Uri(TestSettings.BaseUrl),
            "web/index.php/dashboard/index").ToString();

        DriverManager.Driver.Navigate().GoToUrl(dashboardUrl);

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();

        var recruitmentPage = new RecruitmentPage();
        var vacanciesPage = recruitmentPage.OpenVacancies();
        var addVacancyPage = vacanciesPage.ClickAdd();

        addVacancyPage.EnterVacancyName(_vacancyName);
        addVacancyPage.SelectJobTitle(_jobTitle);
        addVacancyPage.SelectHiringManager(_hiringManager);
        addVacancyPage.Save();

        _editVacancyPage = new EditVacancyPage();
    }

    [Then("the saved vacancy should contain the submitted information")]
    public void ThenSavedVacancyShouldContainSubmittedInformation()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                _editVacancyPage.GetVacancyName(),
                Is.EqualTo(_vacancyName),
                "Vacancy name does not match.");

            Assert.That(
                _editVacancyPage.GetJobTitle(),
                Is.EqualTo(_jobTitle),
                "Job title does not match.");

            Assert.That(
                _editVacancyPage.GetHiringManager(),
                Is.EqualTo(_hiringManager),
                "Hiring manager does not match.");
        });
    }
}
