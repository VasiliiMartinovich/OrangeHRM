using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class ScheduleInterviewPage : LoadablePage
{
    private static readonly By ScheduleInterviewHeader = By.XPath("//h6[text()='Schedule Interview']");
    private static readonly By DateInput =
        By.XPath(
            "//label[normalize-space()='Date']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]//input");
    
    private readonly By _interviewTitleInput =
        By.XPath("//label[text()='Interview Title']/ancestor::div[contains(@class,'oxd-input-group')]//input");
    
    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Interviewer']/ancestor::div[contains(@class,'oxd-input-group')]//input[@placeholder='Type for hints...']")]
    private IWebElement _interviewerInput = null!;
    
    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _saveButton = null!;
    
    public ScheduleInterviewPage() : base(ScheduleInterviewHeader)
    {
        PageFactory.InitElements(Driver, this);
    }
    
    public bool IsDisplayed()
    {
        return FindVisibleElement(ScheduleInterviewHeader).Displayed;
    }
    
    public void SelectInterviewer(string employeeName)
    {
        _interviewerInput.Clear();
        _interviewerInput.SendKeys(employeeName);

        var interviewerOption = By.XPath(
            "//div[contains(@class,'oxd-autocomplete-option') and " +
            $"normalize-space()={ToXPathLiteral(employeeName)}]");

        Wait.Until(driver =>
            driver.FindElements(interviewerOption)
                .Any(element => element.Displayed));

        Click(interviewerOption);
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
    
    public void SelectDate(DateTime date)
    {
        var dateText = date.ToString("yyyy-dd-MM");

        var dateInput = Wait.Until(driver =>
        {
            var element = driver.FindElement(DateInput);

            return element.Displayed && element.Enabled
                ? element
                : null;
        });

        dateInput.Click();
        dateInput.SendKeys(Keys.Control + "a");
        dateInput.SendKeys(dateText);
        dateInput.SendKeys(Keys.Tab);
    }
    
    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();
        return new CandidateDetailsPage();
    }
    
    public ScheduleInterviewPage EnterInterviewTitle(string title)
    {
        var titleInput = FindVisibleElement(_interviewTitleInput);
        titleInput.Clear();
        titleInput.SendKeys(title);

        return this;
    }
    
    public string GetInterviewer()
    {
        var interviewer = _interviewerInput.GetAttribute("value") ?? string.Empty;

        return string.Join(
            " ",
            interviewer.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}