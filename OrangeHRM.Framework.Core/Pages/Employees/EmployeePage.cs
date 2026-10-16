using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;


namespace OrangeHRM.Framework.Core.Pages.Employees;

public class EmployeePage : LoadablePage
{
    private static readonly By EmployeeListHeader = By.CssSelector("a[href='/web/index.php/pim/viewPimModule']");
    private static readonly TextBox _employeeNameInput = new (By.XPath("//label[text()='Employee Name']/../following-sibling::div//input"));
    private readonly Button _searchButton = new(By.CssSelector("button[type='submit']"));
    private static readonly By EmployeeTableRows = By.CssSelector(".oxd-table-body .oxd-table-card");
    private static readonly By SearchSuggestions = By.CssSelector(".oxd-autocomplete-option");
    private readonly Button _resetButton = new(By.CssSelector("button[type='reset']"));
    private static readonly By NoRecordsMessage = By.XPath("//*[normalize-space()='No Records Found']");
    private readonly Button _addEmployeeButton = new(By.XPath("//button[normalize-space()='Add']"));

    public EmployeePage() : base(EmployeeListHeader)
    {
    }
    public bool IsDisplayed()
    {
        return FindVisibleElement(EmployeeListHeader).Displayed;
    }

    public void SearchByEmployeeName(string employeeName)
    {
        _employeeNameInput.SetValue(employeeName);
        SelectFirstEmployeeSuggestion();
        _searchButton.Click();
        WaitForSearchResults();
    }

    public void SearchByNonExistingEmployeeName(string employeeName)
    {
        _employeeNameInput.SetValue(employeeName);
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
                        continue;

                    var text = suggestion.Text.Trim();

                    if (string.IsNullOrWhiteSpace(text) ||
                        text.Contains("Searching",
                            StringComparison.OrdinalIgnoreCase) ||
                        text.Contains("No Records Found",
                            StringComparison.OrdinalIgnoreCase))
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
                    return true;
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
                return Wait.Until(driver =>
                {
                    var rows = driver.FindElements(EmployeeTableRows);
                    var currentCount = 0;

                    try
                    {
                        foreach (var currentRow in rows)
                        {
                            if (currentRow.Displayed &&
                                !currentRow.Text.Contains(
                                    "No Records Found",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                currentCount++;
                            }
                        }

                        return currentCount;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return -1;
                    }
                });
            }
        }

        return count;
    }

    public void ResetSearch()
    {
        _resetButton.Click();

        Wait.Until(driver =>
        {
            try
            {
                var input = driver.FindElement(
                    By.XPath("//label[text()='Employee Name']/../following-sibling::div//input"));

                return string.IsNullOrWhiteSpace(
                    input.GetAttribute("value"));
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
        _addEmployeeButton.Click();
    }
}
