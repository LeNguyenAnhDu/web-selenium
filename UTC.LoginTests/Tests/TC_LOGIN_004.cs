using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using UTC.LoginTests.TestData;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Order(4)]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập")]
public sealed class TC_LOGIN_004 : LoginScenario
{
    [Test]
    public void FakeAccountShowsAuthenticationError()
    {
        const string id = "TC-LOGIN-004";
        var page = OpenPage(id, "Tài khoản giả nhận lỗi xác thực từ server", SeverityLevel.critical);
        var error = SubmitAndWait(page, LoginCases.FakeUsername, LoginCases.FakePassword);
        AllureApi.Step("Kiểm tra lỗi xác thực và form", () =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(error, Is.EqualTo("Tài khoản hoặc mật khẩu không đúng."));
                Assert.That(page.IsFormVisible, Is.True);
            });
        });
    }
}
