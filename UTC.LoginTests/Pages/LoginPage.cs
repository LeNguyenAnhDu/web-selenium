using OpenQA.Selenium;
using UTC.LoginTests.Base;

namespace UTC.LoginTests.Pages;

public sealed class LoginPage : BasePage
{
    private static readonly By Form = By.CssSelector("form[action='/Login'][method='post']");
    private static readonly By Username = By.Name("username");
    private static readonly By Password = By.Name("userpwd");
    private static readonly By Submit = By.CssSelector("form[action='/Login'] input.submit_login[type='submit']");
    private static readonly By ServerError = By.CssSelector("form[action='/Login'] div.error");

    public LoginPage(IWebDriver driver, TimeSpan timeout) : base(driver, timeout) { }

    public LoginPage Open(string url)
    {
        Driver.Navigate().GoToUrl(url);
        FindVisible(Form);
        FindVisible(Username);
        FindVisible(Password);
        return this;
    }

    public void EnterCredentials(string username, string password)
    {
        Enter(Username, username);
        Enter(Password, password);
    }

    public bool HasBrowserValidation => ((IJavaScriptExecutor)Driver).ExecuteScript(
        "return !document.querySelector(\"form[action='/Login']\").checkValidity();") is true;

    public string PasswordInputType => FindVisible(Password).GetAttribute("type") ?? "";

    public bool IsFormVisible => Driver.FindElements(Form).Any(e => e.Displayed);

    public string BodyText => Driver.FindElement(By.TagName("body")).Text;

    public bool HasGuard => Driver.FindElement(By.TagName("body")).Text.Contains("CAPTCHA", StringComparison.OrdinalIgnoreCase)
        || Driver.FindElement(By.TagName("body")).Text.Contains("quá nhiều lần", StringComparison.OrdinalIgnoreCase)
        || Driver.FindElement(By.TagName("body")).Text.Contains("tạm khóa", StringComparison.OrdinalIgnoreCase);

    public void SubmitForm() => Click(Submit);

    public string WaitForServerErrorOrGuard()
    {
        return Wait.Until(d =>
        {
            if (HasGuard) return "__GUARD__";
            var error = d.FindElements(ServerError).FirstOrDefault(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text));
            return error?.Text.Trim();
        })!;
    }
}
