using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Employees;
using Reqnroll;

namespace OrangeHRM.Tests.StepDefinitions.Employees;

[Binding]
public class EmployeeSteps
{
    private EmployeePage _employeePage = null!;

    [When("I open the PIM employee list")]
    public void WhenIOpenThePimEmployeeList()
    {
        var dashboardPage = new DashboardPage();

        dashboardPage.SidePanel.OpenPim();

        _employeePage = new EmployeePage();
    }

    [Then("the employee list page should be displayed")]
    public void ThenTheEmployeeListPageShouldBeDisplayed()
    {
        Assert.That(_employeePage.IsDisplayed(), Is.True,
            "The employee list page should be displayed.");
    }

    [When("I search for employee {string}")]
    public void WhenISearchForEmployee(string employeeName)
    {
        _employeePage.SearchByEmployeeName(employeeName);
    }

    [Then("no employees should be found")]
    public void ThenNoEmployeesShouldBeFound()
    {
        Assert.That(_employeePage.GetSearchResultCount(), Is.EqualTo(0),
            "No employees should be found for a non-existing employee name.");
    }
}