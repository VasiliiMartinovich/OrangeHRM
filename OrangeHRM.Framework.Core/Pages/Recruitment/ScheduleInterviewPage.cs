using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class ScheduleInterviewPage : LoadablePage
{
    private static readonly By ScheduleInterviewHeader = By.XPath("//h6[text()='Schedule Interview']");

    private static readonly By DateInput =
        By.XPath("//label[normalize-space()='Date']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]//input");

    private readonly TextBox _interviewTitleInput = new(
        By.XPath("//label[text()='Interview Title']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

    private readonly TextBox _interviewerInput = new(
        By.XPath("//label[normalize-space()='Interviewer']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]" +
                 "//input[@placeholder='Type for hints...']"));

    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));
    private readonly TextBox _dateInput = new(DateInput);

    public ScheduleInterviewPage() : base(ScheduleInterviewHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(ScheduleInterviewHeader).Displayed;
    }

    public void SelectInterviewer(string employeeName)
    {
        _interviewerInput.Clear();
        _interviewerInput.SetValue(employeeName);

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

        Wait.Until(driver => _dateInput.IsDisplayed() && _dateInput.IsEnabled());

        _dateInput.Click();
        _dateInput.Clear();
        _dateInput.SetValue(dateText);
        _dateInput.SetValue(Keys.Tab);
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }

    public ScheduleInterviewPage EnterInterviewTitle(string title)
    {
        _interviewTitleInput.Clear();
        _interviewTitleInput.SetValue(title);

        return this;
    }

    public string GetInterviewer()
    {
        var interviewer = _interviewerInput.GetAttribute("value") ?? string.Empty;

        return string.Join(" ", interviewer.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
