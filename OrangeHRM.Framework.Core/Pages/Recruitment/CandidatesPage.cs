using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class CandidatesPage : LoadablePage
{
    private static readonly By CandidatesHeader = By.XPath("//h5[normalize-space()='Candidates']");
    private static readonly By TableHeader = By.CssSelector(".oxd-table-header");
    private static readonly By SuccessToast = By.CssSelector(".oxd-toast--success");
    private static readonly By SuccessToastTitle = By.CssSelector(".oxd-toast-content--success .oxd-text--toast-title");
    private static readonly By SuccessToastMessage = By.CssSelector(".oxd-toast-content--success .oxd-text--toast-message");
    private static readonly By ConfirmDeleteButton = By.XPath("//button[normalize-space()='Yes, Delete']");
    private readonly Button _addButton = new(By.XPath("//button[normalize-space()='Add']"));
    private readonly Button _searchButton = new(By.XPath("//button[normalize-space()='Search']"));
    private readonly TextBox _candidateNameFilter = new(By.XPath("//label[normalize-space()='Candidate Name']" +
                                                                 "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

    public CandidatesPage() : base(CandidatesHeader)
    {
    }

    public AddCandidatePage ClickAdd()
    {
        Wait.Until(driver => _addButton.IsDisplayed() && _addButton.IsEnabled());

        _addButton.Click();

        return new AddCandidatePage();
    }

    public void DeleteCandidate(string candidateName)
    {
        var deleteButton = DeleteButton(candidateName);

        Click(deleteButton);
        Click(ConfirmDeleteButton);
    }

    public bool IsCandidateDisplayed(string candidateName)
    {
        return Driver.FindElements(CandidateRow(candidateName)).Any(element => element.Displayed);
    }

    public bool IsSuccessToastDisplayed()
    {
        return FindVisibleElement(SuccessToast).Displayed;
    }

    public string GetSuccessToastTitle()
    {
        return FindVisibleElement(SuccessToastTitle).Text.Trim();
    }

    public string GetSuccessToastMessage()
    {
        return FindVisibleElement(SuccessToastMessage).Text.Trim();
    }

    public void SearchCandidate(string candidateName)
    {
        _candidateNameFilter.Clear();
        _candidateNameFilter.SetValue(candidateName);

        Wait.Until(driver => _searchButton.IsDisplayed() && _searchButton.IsEnabled());

        _searchButton.Click();

        Wait.Until(driver => driver.FindElements(TableHeader).Any(element => element.Displayed));
    }

    private static By CandidateRow(string candidateName)
    {
        return By.XPath(
            "//div[contains(@class,'oxd-table-row')]" +
            "[.//div[contains(@class,'oxd-table-cell')]" +
            "[contains(normalize-space(.), " +
            ToXPathLiteral(candidateName) + ")]]");
    }

    private static By DeleteButton(string candidateName)
    {
        return By.XPath(
            "//div[contains(@class,'oxd-table-row')]" +
            "[.//div[contains(@class,'oxd-table-cell')]" +
            "[contains(normalize-space(.), " +
            ToXPathLiteral(candidateName) + ")]]" +
            "//button[.//*[contains(@class,'bi-trash') " +
            "or contains(@class,'oxd-icon-trash')]]");
    }

    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }

        if (!value.Contains('"'))
        {
            return $"\"{value}\"";
        }

        var parts = value.Split('\'');

        return "concat('" +
               string.Join("', \"'\", '", parts) +
               "')";
    }
}
