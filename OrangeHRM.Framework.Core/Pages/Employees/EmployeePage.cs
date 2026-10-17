using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class EmployeePage : LoadablePage
{
    private static readonly By EmployeeListHeader = By.CssSelector("a[href='/web/index.php/pim/viewPimModule']");
    private static readonly By EmployeeTableRows = By.CssSelector(".oxd-table-body .oxd-table-card");
    private static readonly By SearchSuggestions = By.CssSelector(".oxd-autocomplete-option");
    private static readonly By NoRecordsMessage = By.XPath("//*[normalize-space()='No Records Found']");

    [FindsBy(
        How = How.XPath,
        Using = "//label[text()='Employee Name']/../following-sibling::div//input")]
    private IWebElement _employeeNameInput = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _searchButton = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='reset']")]
    private IWebElement _resetButton = null!;

    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Add']")]
    private IWebElement _addEmployeeButton = null!;

    public EmployeePage() : base(EmployeeListHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(EmployeeListHeader).Displayed;
    }

    public void SearchByEmployeeName(string employeeName)
    {
        Wait.Until(driver =>
            _employeeNameInput.Displayed && _employeeNameInput.Enabled);

        _employeeNameInput.Clear();
        _employeeNameInput.SendKeys(employeeName);

        SelectFirstEmployeeSuggestion();

        Wait.Until(driver =>
            _searchButton.Displayed && _searchButton.Enabled);

        _searchButton.Click();

        WaitForSearchResults();
    }

    public void SearchByNonExistingEmployeeName(string employeeName)
    {
        Wait.Until(driver =>
            _employeeNameInput.Displayed && _employeeNameInput.Enabled);

        _employeeNameInput.Clear();
        _employeeNameInput.SendKeys(employeeName);

        Wait.Until(driver => _searchButton.Displayed && _searchButton.Enabled);

        _searchButton.Click();

        Wait.Until(driver => HasNoRecordsMessage(driver));
    }

    public void SelectFirstEmployeeSuggestion()
    {
        Wait.Until(driver =>
        {
            foreach (var suggestion in driver.FindElements(SearchSuggestions))
            {
                try
                {
                    if (!suggestion.Displayed)
                    {
                        continue;
                    }

                    var text = suggestion.Text.Trim();

                    if (string.IsNullOrWhiteSpace(text) ||
                        text.Contains("Searching", StringComparison.OrdinalIgnoreCase) ||
                        text.Contains("No Records Found", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    suggestion.Click();
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    
                }
                catch (ElementClickInterceptedException)
                {
                    
                }
            }

            return false;
        });
    }

    private void WaitForSearchResults()
    {
        Wait.Until(driver =>
            HasVisibleEmployeeRows(driver) ||
            HasNoRecordsMessage(driver));
    }

    private bool HasVisibleEmployeeRows(IWebDriver driver)
    {
        foreach (var row in driver.FindElements(EmployeeTableRows))
        {
            try
            {
                if (row.Displayed &&
                    !row.Text.Contains(
                        "No Records Found",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }

        return false;
    }

    private bool HasNoRecordsMessage(IWebDriver driver)
    {
        foreach (var element in driver.FindElements(NoRecordsMessage))
        {
            try
            {
                if (element.Displayed)
                {
                    return true;
                }
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }

        return false;
    }

    
    public int GetSearchResultCount()
    {
        Wait.Until(driver =>
        {
            try
            {
                foreach (var row in driver.FindElements(EmployeeTableRows))
                {
                    _ = row.Displayed;
                }

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });

        var count = 0;

        foreach (var row in Driver.FindElements(EmployeeTableRows))
        {
            try
            {
                if (row.Displayed &&
                    !row.Text.Contains(
                        "No Records Found",
                        StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }
            catch (StaleElementReferenceException)
            {
                return GetSearchResultCount();
            }
        }

        return count;
    }


    public void ResetSearch()
    {
        Wait.Until(driver =>
            _resetButton.Displayed && _resetButton.Enabled);

        _resetButton.Click();

        Wait.Until(driver =>
        {
            try
            {
                return string.IsNullOrWhiteSpace(
                    _employeeNameInput.GetAttribute("value"));
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });

        WaitForSearchResults();
    }

    public int WaitForEmployeeList()
    {
        WaitForSearchResults();
        return GetSearchResultCount();
    }

    public int WaitForSearchResultCount()
    {
        WaitForSearchResults();
        return GetSearchResultCount();
    }

    public void ClickAddEmployee()
    {
        Wait.Until(driver =>
            _addEmployeeButton.Displayed && _addEmployeeButton.Enabled);

        _addEmployeeButton.Click();
    }
}
