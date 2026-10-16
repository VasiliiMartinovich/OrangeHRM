using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Pages.Recruitment;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddCandidatePage : LoadablePage
{
    private static readonly By AddCandidateHeader = By.XPath("//h6[text()='Add Candidate']");
    private static readonly By FirstNameInput = By.Name("firstName");
    private static readonly By MiddleNameInput = By.Name("middleName");
    private static readonly By LastNameInput = By.Name("lastName");
    private static readonly By VacancyDropdown =
        By.XPath(
            "//label[normalize-space()='Vacancy']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]" +
            "//div[contains(@class,'oxd-select-text')]");

    private static readonly By EmailInput = By.XPath("//label[normalize-space()='Email']/../following-sibling::div//input");
    private static readonly By CvInput = By.XPath("//input[@type='file']");
    private static readonly By SaveButton = By.CssSelector("button[type='submit']");
    
    
    public AddCandidatePage() : base(AddCandidateHeader)
    {
    }
    public void FillCandidateForm(
        string firstName,
        string middleName,
        string lastName,
        string email)
    {
        TypeText(FirstNameInput, firstName);
        TypeText(MiddleNameInput, middleName);
        TypeText(LastNameInput, lastName);
        TypeText(EmailInput, email);
    }

    public void SelectVacancy(string vacancyName)
    {
        Click(VacancyDropdown);

        var options = By.XPath(
            "//div[contains(@class,'oxd-select-option')]");

        Wait.Until(driver =>
            driver.FindElements(options)
                .Any(element => element.Displayed));

        var visibleOptions = Driver.FindElements(options)
            .Where(element => element.Displayed)
            .Select(element => element.Text.Trim())
            .ToList();

        Console.WriteLine(
            $"Requested vacancy: {vacancyName}");

        Console.WriteLine(
            $"Available vacancies: {string.Join(", ", visibleOptions)}");

        var vacancyOption = By.XPath(
            "//div[contains(@class,'oxd-select-option')]" +
            $"[normalize-space()={ToXPathLiteral(vacancyName)}]");

        Click(vacancyOption);
    }
    
    public void UploadCv(string cvFilePath)
    {
        Driver.FindElement(CvInput).SendKeys(cvFilePath);
    }

    public bool IsCvSelected(string fileName)
    {
        var value = Driver.FindElement(CvInput).GetAttribute("value");

        return !string.IsNullOrEmpty(value) &&
               value.EndsWith(fileName, StringComparison.OrdinalIgnoreCase);
    }

    public CandidateDetailsPage Save()
    {
        Click(SaveButton);
        return new CandidateDetailsPage();
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