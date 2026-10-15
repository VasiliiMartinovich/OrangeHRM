using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.UI.Pages;
using OrangeHRM.Framework.UI.Pages.Authentication;
using OrangeHRM.Framework.UI.Pages.Employees;
using OrangeHRM.Framework.UI.Pages.Components;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.UI.Employees;

[TestFixture]
public class EmployeeTests : UiTestBase
{
    [SetUp]
    public void SetUp()
    {
        var loginPage = new LoginPage();
        loginPage.Login("Admin", "admin123");
    }
    
    [Test]
    public void OpenEmployeeListPage()
    {
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();
        var employeePage = new EmployeePage();
        Assert.That(employeePage.IsDisplayed(), Is.True, "Employee Information should be displayed.");
    }
    
    [Test]
    public void SearchEmployeeWithExistingName()
    {
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();
    
        var employeePage = new EmployeePage();
        employeePage.SearchByEmployeeName("John");
    
        var count = employeePage.GetSearchResultCount();

        Console.WriteLine($"Search result count: {count}");

        Assert.That(count, Is.GreaterThan(0),
            "At least one employee should be displayed after searching.");
    }

    [Test]
    public void SearchEmployeeWithNonExistingName()
    {
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();
        var employeePage = new EmployeePage();
        employeePage.SearchByNonExistingEmployeeName("NonExistingEmployee123");
        Assert.That(employeePage.GetSearchResultCount(), Is.EqualTo(0),
                "No employees should be displayed for a non-existing name.");
    }
    
    [Test]
    public void ResetSearchShouldClearFilters()
    {
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();
        var employeePage = new EmployeePage();
        var initialCount = employeePage.WaitForEmployeeList();
        employeePage.SearchByEmployeeName("John");
        var searchCount = employeePage.WaitForSearchResultCount();
        Assert.That(searchCount, Is.GreaterThan(0),
            "Search should return at least one employee.");
        employeePage.ResetSearch();
        var resetCount = employeePage.WaitForEmployeeList();
        Assert.That(resetCount, Is.EqualTo(initialCount),
            "Reset should restore the initial employee list.");
    }
    
    [Test]
    public void AddEmployee()
    {
        var dashboardPage = new DashboardPage();
        dashboardPage.SidePanel.OpenPim();
        var employeePage = new EmployeePage();
        employeePage.ClickAddEmployee();
        var addEmployeePage = new AddEmployeePage();
        var firstName = "Test" + DateTime.Now.ToString("HHmmss");
        var middleName = "Ivanovich";
        var lastName = "Employee";
        addEmployeePage.CreateEmployee(
            firstName: firstName,
            lastName: lastName,
            middleName: middleName);
        var personalDetailsPage = new PersonalDetailsPage();
        Assert.That(personalDetailsPage.IsDisplayed(), Is.True,
            "Personal Details page should be displayed after saving.");
        var actualFirstName = personalDetailsPage.WaitForFirstName();
        var actualLastName = personalDetailsPage.GetLastName();
        Assert.That(actualFirstName, Is.EqualTo(firstName),
            "First Name should match the value entered during employee creation.");
        Assert.That(actualLastName, Is.EqualTo(lastName),
            "Last Name should match the value entered during employee creation.");
    }
}