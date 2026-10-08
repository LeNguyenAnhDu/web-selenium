using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace UTC.LoginTests.Tests;

[TestFixture, NonParallelizable, Order(5)]
[AllureNUnit, AllureEpic("Văn phòng điện tử UTC"), AllureFeature("Đăng nhập")]
public sealed class TC_LOGIN_005 : LoginScenario
{
    [Test]
    public void WhitespaceFieldsShowAuthenticationError()
    {
        const string id = "TC-LOGIN-005";
        var page = OpenPage(id, "Hai trường chỉ chứa khoảng trắng nhận lỗi xác thực", SeverityLevel.normal);
        var error = SubmitAndWait(page, "   ", "   ");
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
