using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Employees;
using OrangeHRM.Framework.Core.Pages.Recruitment;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.Infrastructure;

public class TestDataSetup
{
    public string CreateEmployeeForHiringManager()
    {
        var uniqueId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        var firstName = $"Manager{uniqueId}";
        var middleName = "Test";
        var lastName = $"Employee{uniqueId}";

        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();

        var employeePage = new EmployeePage();
        employeePage.ClickAddEmployee();

        var addEmployeePage = new AddEmployeePage();

        addEmployeePage.CreateEmployee(
            firstName: firstName,
            lastName: lastName,
            middleName: middleName);

        var personalDetailsPage = new PersonalDetailsPage();

        if (!personalDetailsPage.IsDisplayed())
        {
            throw new InvalidOperationException(
                "Employee was not created successfully.");
        }

        return $"{firstName} {middleName} {lastName}";
    }
    
    public string CreateVacancyForHiringManager()
    {
        var hiringManager = CreateEmployeeForHiringManager();

        DriverManager.Driver.Navigate().GoToUrl(new Uri(new Uri(TestSettings.BaseUrl),
                "web/index.php/dashboard/index").ToString());

        var vacancyName = $"QA Automation Vacancy {DateTime.Now:yyyyMMddHHmmssfff}";
        var jobTitle = "Software Engineer";
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenRecruitment();
        var recruitmentPage = new RecruitmentPage();
        recruitmentPage.OpenVacancies();
        var vacanciesPage = new VacanciesPage();
        vacanciesPage.ClickAdd();
        var addVacancyPage = new AddVacancyPage();
        addVacancyPage.EnterVacancyName(vacancyName);
        addVacancyPage.SelectJobTitle(jobTitle);
        addVacancyPage.SelectHiringManager(hiringManager);
        addVacancyPage.Save();

        return vacancyName;
    }
}