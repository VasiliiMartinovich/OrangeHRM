using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class VacanciesPage : LoadablePage
{
    private static readonly By VacanciesHeader = By.XPath("//h5[normalize-space()='Vacancies']");
    private static readonly By AddVacanciesButton = By.XPath("//button[normalize-space()='Add']");
    public VacanciesPage() : base(VacanciesHeader)
    {
    }
    
    public AddVacancyPage ClickAdd()
    {
        Click(AddVacanciesButton);
        return new AddVacancyPage();
    }
}