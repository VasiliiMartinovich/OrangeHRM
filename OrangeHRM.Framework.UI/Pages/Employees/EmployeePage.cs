using OpenQA.Selenium;
using System.Linq;
using OrangeHRM.Framework.UI.Pages;

namespace OrangeHRM.Framework.UI.Pages.Employees;

public class EmployeePage : LoadablePage
{
    private static readonly By EmployeeListHeader = By.CssSelector("a[href='/web/index.php/pim/viewPimModule']");
    private static readonly By EmployeeNameInput = By.XPath("//label[text()='Employee Name']/../following-sibling::div//input");
    private static readonly By SearchButton = By.CssSelector("button[type='submit']");
    private static readonly By EmployeeTableRows = By.CssSelector(".oxd-table-body .oxd-table-card");
    private static readonly By SearchSuggestions = By.CssSelector(".oxd-autocomplete-option");
    private static readonly By ResetButton = By.CssSelector("button[type='reset']");
    private static readonly By NoRecordsMessage = By.XPath("//*[normalize-space()='No Records Found']");
    private static readonly By AddEmployeeButton = By.XPath("//button[normalize-space()='Add']");
    
    public EmployeePage() : base(EmployeeListHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(EmployeeListHeader).Displayed;
    }

    public void SearchByEmployeeName(string employeeName)
    {
        TypeText(EmployeeNameInput, employeeName);
        SelectFirstEmployeeSuggestion();
        Click(SearchButton);
        WaitForSearchResults();
    }

   public void SearchByNonExistingEmployeeName(string employeeName)
    {
        TypeText(EmployeeNameInput, employeeName);
        Click(SearchButton);
        Wait.Until(driver =>
        {
            var noRecordsFound = driver.FindElements(NoRecordsMessage);
            return noRecordsFound.Any(element =>
            {
                try
                {
                    return element.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        });
    }

    public void SelectFirstEmployeeSuggestion()
    {
        var suggestion = Wait.Until(driver => FindFirstUsableSuggestion(driver));
        suggestion.Click();
    }

    private IWebElement? FindFirstUsableSuggestion(IWebDriver driver)
    {
        foreach (var element in driver.FindElements(SearchSuggestions))
        {
            try
            {
                if (!element.Displayed)
                    continue;

                var text = element.Text.Trim();

                if (string.IsNullOrWhiteSpace(text))
                    continue;
                if (text.Contains("Searching"))
                    continue;
                if (text.Contains("No Records Found"))
                    continue;
                return element;
            }
            catch (StaleElementReferenceException)
            {
            }
        }
        return null;
    }

    private void WaitForSearchResults()
    {
        Wait.Until(driver =>
        {
            var rows = driver.FindElements(EmployeeTableRows);
            return rows.Any(row =>
            {
                try
                {
                    return row.Displayed &&
                           !row.Text.Contains("No Records Found");
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            }) ||
            driver.FindElements(NoRecordsMessage)
                .Any(element =>
                {
                    try
                    {
                        return element.Displayed;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                });
        });
    }
    public int GetSearchResultCount()
    {
        return Driver.FindElements(EmployeeTableRows)
            .Count(row =>
            {
                try
                {
                    return row.Displayed &&
                           !row.Text.Contains("No Records Found");
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
    }
    public void ResetSearch()
    {
        Click(ResetButton);
       Wait.Until(driver => string.IsNullOrWhiteSpace(driver.FindElement(EmployeeNameInput).GetAttribute("value")));
        
        Wait.Until(driver => driver.FindElements(EmployeeTableRows).Any(row =>
                {
                    try
                    {
                        return row.Displayed &&
                               !row.Text.Contains("No Records Found");
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                }));
    }
    public int WaitForEmployeeList()
    {
        Wait.Until(driver =>
            driver.FindElements(EmployeeTableRows)
                .Any(row =>
                {
                    try
                    {
                        return row.Displayed &&
                               !row.Text.Contains("No Records Found");
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                }));
        return GetSearchResultCount();
    }
    
    public int WaitForSearchResultCount()
    {
        Wait.Until(driver =>
        {
            var rows = driver.FindElements(EmployeeTableRows);

            return rows.Any(row =>
            {
                try
                {
                    return row.Displayed &&
                           !row.Text.Contains("No Records Found");
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        });
        return GetSearchResultCount();
    }
    public void ClickAddEmployee()
    {
        Click(AddEmployeeButton);
    }
}