using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using UTC.LoginTests.TestData;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Order(3)]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập")]
public sealed class TC_LOGIN_003 : LoginScenario
{
    [TestCase("TC-LOGIN-003", TestName = "TC-LOGIN-003_UsernameEmpty")]
    public void UsernameEmptyShowsServerValidation(string id)
    {
        var data = LoginCases.ForId(id);
        var page = OpenPage(id, "Bỏ trống username, có mật khẩu giả", SeverityLevel.critical);
        var error = SubmitAndWait(page, data.Username, data.Password);
        AllureApi.Step("Kiểm tra validation server và form", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(error, Is.EqualTo(data.ExpectedError));
                Assert.That(page.HasBrowserValidation, Is.False);
                Assert.That(page.IsFormVisible, Is.True);
            });
        });
    }
}
