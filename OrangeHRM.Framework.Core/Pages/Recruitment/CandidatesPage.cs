using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace OrangeHRM.Framework.UI.Pages.Recruitment;

public class CandidatesPage : LoadablePage
{
    private static readonly By CandidatesHeader = By.XPath("//h5[normalize-space()='Candidates']");
    private static readonly By AddButton = By.XPath("//button[normalize-space()='Add']");
    private static readonly By CandidateNameFilter =
        By.XPath(
            "//label[normalize-space()='Candidate Name']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]//input");

    private static readonly By SearchButton = By.XPath("//button[normalize-space()='Search']");
    private static readonly By TableHeader = By.CssSelector(".oxd-table-header");
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
    private static readonly By ConfirmDeleteButton = By.XPath("//button[normalize-space()='Yes, Delete']");
    private static readonly By SuccessToast = By.CssSelector(".oxd-toast--success");
    private static readonly By SuccessToastTitle = By.CssSelector(".oxd-toast-content--success .oxd-text--toast-title");
    private static readonly By SuccessToastMessage = By.CssSelector(".oxd-toast-content--success .oxd-text--toast-message");
    public CandidatesPage() : base(CandidatesHeader)
    {
    }
    public AddCandidatePage ClickAdd()
    {
        Click(AddButton);
        return new AddCandidatePage();
    }
    public void DeleteCandidate(string candidateName)
    {
        Click(DeleteButton(candidateName));
        Click(ConfirmDeleteButton);
    }
    public bool IsCandidateDisplayed(string candidateName)
    {
        return Driver.FindElements(CandidateRow(candidateName))
            .Any(element => element.Displayed);
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
    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains("'"))
            return $"'{value}'";

        if (!value.Contains("\""))
            return $"\"{value}\"";

        var parts = value.Split('\'');
        return "concat('" +
               string.Join("', \"'\", '", parts) +
               "')";
    }
    public void SearchCandidate(string candidateName)
    {
        var searchInput = FindVisibleElement(CandidateNameFilter);
        searchInput.Clear();
        searchInput.SendKeys(candidateName);
        Click(SearchButton);
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(driver => driver.FindElements(TableHeader).Any(element => element.Displayed));
    }
}