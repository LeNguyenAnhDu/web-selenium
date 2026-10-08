using Allure.Net.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UTC.LoginTests.Configuration;
using UTC.LoginTests.Helpers;

namespace UTC.LoginTests.Base;

public abstract class BaseTest
{
    protected IWebDriver Driver = null!;
    protected TestSettings Settings = null!;

    [SetUp]
    public void SetUpBrowser()
    {
        Settings = TestSettings.Load();
        var options = new ChromeOptions();
        if (Settings.Headless) options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,900");
        Driver = new ChromeDriver(options);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        TestContext.Progress.WriteLine($"Chrome started; headless={Settings.Headless}; URL={Settings.Url}");
    }

    [TearDown]
    public void TearDownBrowser()
    {
        if (Driver is null) return;
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed && Driver is ITakesScreenshot camera)
            {
                try
                {
                    var directory = Environment.GetEnvironmentVariable("UTC_EVIDENCE_DIR")
                        ?? Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
                    Directory.CreateDirectory(directory);
                    var id = TestCaseIds.FromTestName(TestContext.CurrentContext.Test.Name);
                    var path = Path.Combine(directory, $"{id}_{Guid.NewGuid():N}.png");
                    camera.GetScreenshot().SaveAsFile(path);
                    TestContext.AddTestAttachment(path, "Screenshot on failure");
                    AllureApi.AddAttachment("Screenshot on failure", "image/png", path);
                    TestContext.Progress.WriteLine($"Screenshot: {path}");
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"Screenshot failed: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
        finally
        {
            try { Driver.Quit(); }
            catch (Exception ex) { TestContext.Progress.WriteLine($"Chrome quit failed: {ex.Message}"); }
            Driver.Dispose();
        }
    }
}
