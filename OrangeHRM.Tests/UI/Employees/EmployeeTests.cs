using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using log4net;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Employees;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.UI.Employees;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class EmployeeTests : UiTestBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(EmployeeTests));

    [SetUp]
    public void SetUp()
    {
        Log.Info("EmployeeTests setup: logging in to OrangeHRM.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        AllureApi.Step(
            "Log in with configured valid credentials",
            () => loginPage.Login(
                TestSettings.Username,
                TestSettings.Password));

        Log.Info("EmployeeTests setup: login completed.");
    }

    [Test]
    [AllureName("Employees - open Employee List page")]
    [AllureTag("UI",  "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that an authenticated user can open the PIM module and display the Employee Information page.")]
    public void OpenEmployeeListPage()
    {
        Log.Info("Test started: OpenEmployeeListPage.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening PIM from the side panel.");

        AllureApi.Step(
            "Open PIM from the side panel",
            () => dashboardPage.SidePanel.OpenPim());

        Log.Info("Checking that the Employee Information page is displayed.");

        var employeePage = AllureApi.Step(
            "Create EmployeePage object",
            () => new EmployeePage());

        AllureApi.Step("Verify Employee Information page is displayed", () =>
        {
            Assert.That(employeePage.IsDisplayed(), Is.True,
                "Employee Information should be displayed.");
        });

        Log.Info("Test passed: Employee Information page is displayed.");
    }

    [Test]
    [AllureName("Employees - search by existing name")]
    [AllureTag("UI", "Employees")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that searching for employees by the existing name John returns at least one employee.")]
    public void SearchEmployeeWithExistingName()
    {
        Log.Info("Test started: SearchEmployeeWithExistingName.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening PIM from the side panel.");

        AllureApi.Step(
            "Open PIM from the side panel",
            () => dashboardPage.SidePanel.OpenPim());

        var employeePage = AllureApi.Step(
            "Create EmployeePage object",
            () => new EmployeePage());

        Log.Info("Searching employees by name: John.");

        AllureApi.Step(
            "Search employees by the name John",
            () => employeePage.SearchByEmployeeName("John"));

        var count = AllureApi.Step(
            "Get employee search result count",
            () => employeePage.GetSearchResultCount());

        Log.Info($"Search completed. Result count: {count}.");

        AllureApi.Step("Verify search returned at least one employee", () =>
        {
            Assert.That(count, Is.GreaterThan(0),
                "At least one employee should be displayed after searching.");
        });

        Log.Info("Test passed: search returned at least one employee.");
    }

    [Test]
    [AllureName("Employees - search by non-existing name")]
    [AllureTag("UI", "Employees")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that searching for a non-existing employee name returns zero search results.")]
    public void SearchEmployeeWithNonExistingName()
    {
        Log.Info("Test started: SearchEmployeeWithNonExistingName.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening PIM from the side panel.");

        AllureApi.Step(
            "Open PIM from the side panel",
            () => dashboardPage.SidePanel.OpenPim());

        var employeePage = AllureApi.Step(
            "Create EmployeePage object",
            () => new EmployeePage());

        const string nonExistingName = "NonExistingEmployee123";

        Log.Info($"Searching employees by non-existing name: {nonExistingName}.");

        AllureApi.Step(
            "Search by a non-existing employee name",
            () => employeePage.SearchByNonExistingEmployeeName(nonExistingName));

        var count = AllureApi.Step(
            "Get employee search result count",
            () => employeePage.GetSearchResultCount());

        Log.Info($"Search completed. Result count: {count}.");

        AllureApi.Step("Verify no employees were returned", () =>
        {
            Assert.That(count, Is.EqualTo(0),
                "No employees should be displayed for a non-existing name.");
        });

        Log.Info("Test passed: search returned no employees.");
    }

    [Test]
    [NonParallelizable]
    [AllureName("Employees - reset search filters")]
    [AllureTag("UI", "Employees")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that searching by an existing employee name filters the list and resetting the search restores the initial employee count.")]
    public void ResetSearchShouldClearFilters()
    {
        Log.Info("Test started: ResetSearchShouldClearFilters.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening PIM from the side panel.");

        AllureApi.Step(
            "Open PIM from the side panel",
            () => dashboardPage.SidePanel.OpenPim());

        var employeePage = AllureApi.Step(
            "Create EmployeePage object",
            () => new EmployeePage());

        Log.Info("Waiting for the initial employee list.");

        var initialCount = AllureApi.Step(
            "Wait for the initial employee list",
            () => employeePage.WaitForEmployeeList());

        Log.Info($"Initial employee count: {initialCount}.");

        Log.Info("Searching employees by name: John.");

        AllureApi.Step(
            "Search employees by the name John",
            () => employeePage.SearchByEmployeeName("John"));

        var searchCount = AllureApi.Step(
            "Wait for search results",
            () => employeePage.WaitForSearchResultCount());

        Log.Info($"Search result count: {searchCount}.");

        AllureApi.Step("Verify search returned at least one employee", () =>
        {
            Assert.That(searchCount, Is.GreaterThan(0),
                "Search should return at least one employee.");
        });

        Log.Info("Resetting employee search filters.");

        AllureApi.Step(
            "Reset employee search filters",
            () => employeePage.ResetSearch());

        var resetCount = AllureApi.Step(
            "Wait for the employee list after reset",
            () => employeePage.WaitForEmployeeList());

        Log.Info($"Employee count after reset: {resetCount}.");

        AllureApi.Step("Verify reset restores the initial employee count", () =>
        {
            Assert.That(resetCount, Is.EqualTo(initialCount),
                "Reset should restore the initial employee list.");
        });

        Log.Info("Test passed: resetting search restored the initial list.");
    }

    [Test]
    [AllureName("Employees - create new employee")]
    [AllureTag("UI", "Employees")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that a new employee can be created with generated employee details and that the Personal Details page displays the submitted first and last names.")]
    public void AddEmployee()
    {
        Log.Info("Test started: AddEmployee.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening PIM from the side panel.");

        AllureApi.Step(
            "Open PIM from the side panel",
            () => dashboardPage.SidePanel.OpenPim());

        var employeePage = AllureApi.Step(
            "Create EmployeePage object",
            () => new EmployeePage());

        Log.Info("Opening the Add Employee page.");

        AllureApi.Step(
            "Open Add Employee page",
            () => employeePage.ClickAddEmployee());

        var addEmployeePage = AllureApi.Step(
            "Create AddEmployeePage object",
            () => new AddEmployeePage());

        var firstName = "Test" + DateTime.Now.ToString("HHmmss");
        const string middleName = "Ivanovich";
        const string lastName = "Employee";
        var employeeId = DateTime.Now.ToString("HHmmss");

        Log.Info(
            $"Creating employee with first name: {firstName}, last name: {lastName}.");

        AllureApi.Step("Fill in and submit employee information", () =>
        {
            addEmployeePage.CreateEmployee(
                firstName: firstName,
                lastName: lastName,
                middleName: middleName,
                employeeId: employeeId);
        });

        Log.Info("Employee creation submitted. Checking the Personal Details page.");

        var personalDetailsPage = AllureApi.Step(
            "Create PersonalDetailsPage object",
            () => new PersonalDetailsPage());

        AllureApi.Step("Verify Personal Details page is displayed", () =>
        {
            Assert.That(personalDetailsPage.IsDisplayed(), Is.True,
                "Personal Details page should be displayed after saving.");
        });

        Log.Info("Personal Details page is displayed.");

        var actualFirstName = AllureApi.Step(
            "Read saved employee first name",
            () => personalDetailsPage.WaitForFirstName());

        var actualLastName = AllureApi.Step(
            "Read saved employee last name",
            () => personalDetailsPage.GetLastName());

        Log.Info($"Verifying saved employee details for first name: {firstName}.");

        AllureApi.Step("Verify saved employee name fields", () =>
        {
            Assert.That(actualFirstName, Is.EqualTo(firstName),
                "First Name should match the value entered during employee creation.");

            Assert.That(actualLastName, Is.EqualTo(lastName),
                "Last Name should match the value entered during employee creation.");
        });

        Log.Info("Test passed: employee was created and the name fields match.");
    }
}