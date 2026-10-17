using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class VacanciesPage : LoadablePage
{
    private static readonly By VacanciesHeader = By.XPath("//h5[normalize-space()='Vacancies']");

    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Add']")]
    private IWebElement _addVacanciesButton = null!;

    public VacanciesPage() : base(VacanciesHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public AddVacancyPage ClickAdd()
    {
        _addVacanciesButton.Click();

        return new AddVacancyPage();
    }
}