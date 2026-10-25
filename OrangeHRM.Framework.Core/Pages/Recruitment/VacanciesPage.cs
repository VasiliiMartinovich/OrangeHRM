using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class VacanciesPage : LoadablePage
{
    private static readonly By VacanciesHeader = By.XPath("//h5[normalize-space()='Vacancies']");
    private readonly Button _addVacanciesButton = new(By.XPath("//button[normalize-space()='Add']"));

    public VacanciesPage() : base(VacanciesHeader)
    {
    }

    public AddVacancyPage ClickAdd()
    {
        _addVacanciesButton.Click();

        return new AddVacancyPage();
    }
}