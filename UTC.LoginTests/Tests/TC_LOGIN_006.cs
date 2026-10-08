using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using UTC.LoginTests.TestData;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Order(6)]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập")]
public sealed class TC_LOGIN_006 : LoginScenario
{
    [Test]
    public void PasswordInputIsMasked()
    {
        const string id = "TC-LOGIN-006";
        var page = OpenPage(id, "Mật khẩu được che bằng input type=password", SeverityLevel.normal);
        AllureApi.Step("Nhập mật khẩu giả", () => page.EnterCredentials("", LoginCases.FakePassword));
        AllureApi.Step("Kiểm tra kiểu ô mật khẩu", () =>
            Assert.That(page.PasswordInputType, Is.EqualTo("password")));
    }
}
