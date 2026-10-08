using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using UTC.LoginTests.TestData;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Order(2)]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập")]
public sealed class TC_LOGIN_002 : LoginScenario
{
    [TestCase("TC-LOGIN-002", TestName = "TC-LOGIN-002_PasswordEmpty")]
    public void PasswordEmptyShowsServerValidation(string id)
    {
        var data = LoginCases.ForId(id);
        var page = OpenPage(id, "Có username giả, bỏ trống mật khẩu", SeverityLevel.critical);
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
