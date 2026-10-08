using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UTC.LoginTests.Base;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver, TimeSpan timeout)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, timeout);
        Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
    }

    protected IWebElement FindVisible(By locator) => Wait.Until(d =>
    {
        var element = d.FindElements(locator).FirstOrDefault(e => e.Displayed);
        return element;
    })!;

    protected void Enter(By locator, string value)
    {
        var element = FindVisible(locator);
        element.Clear();
        element.SendKeys(value);
    }

    protected void Click(By locator) => FindVisible(locator).Click();

    protected string Read(By locator) => FindVisible(locator).Text.Trim();
}
