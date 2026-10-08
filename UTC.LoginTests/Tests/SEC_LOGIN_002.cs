using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using UTC.LoginTests.Configuration;
using UTC.LoginTests.TestData;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Category("Security")]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập"), AllureStory("SQL injection probe")]
public sealed class SEC_LOGIN_002 : LoginScenario
{
    [TestCase("SEC-LOGIN-002", TestName = "SEC-LOGIN-002_PasswordQuote")]
    public void PasswordQuoteDoesNotExposeSqlErrorOrAuthenticate(string id)
    {
        var stagingUrl = SecurityTarget.RequireStagingUrl();
        var page = OpenPage(id, "Dấu nháy đơn trong password không gây lỗi SQL lộ ra giao diện", SeverityLevel.critical, stagingUrl);
        var error = SubmitAndWait(page, LoginCases.FakeUsername, "'");
        AllureApi.Step("Kiểm tra form còn hiển thị và không lộ lỗi SQL", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(error, Is.Not.Empty);
                Assert.That(page.IsFormVisible, Is.True);
                Assert.That(page.BodyText, Does.Not.Match(@"(?i)(SqlException|SQL syntax|ORA-\d{5}|ODBC|PostgreSQL|MySqlException)"));
            });
        });
    }
}
