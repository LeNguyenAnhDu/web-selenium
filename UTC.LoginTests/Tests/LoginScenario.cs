using Allure.Net.Commons;
using NUnit.Framework;
using UTC.LoginTests.Base;
using UTC.LoginTests.Helpers;
using UTC.LoginTests.Pages;

namespace UTC.LoginTests.Tests;

public abstract class LoginScenario : BaseTest
{
    protected LoginPage OpenPage(string id, string description, SeverityLevel severity)
    {
        AllureApi.SetTestName($"{id} — {description}");
        AllureApi.SetDescription(description);
        AllureApi.SetSeverity(severity);
        AllureApi.AddLabel("testCaseId", id);
        return AllureApi.Step("Mở trang đăng nhập", () =>
            new LoginPage(Driver, TimeSpan.FromSeconds(Settings.TimeoutSeconds)).Open(Settings.Url));
    }

    protected string SubmitAndWait(LoginPage page, string username, string password)
    {
        if (SubmissionGuard.IsBlocked || page.HasGuard)
        {
            SubmissionGuard.Block();
            Assert.Inconclusive("Blocked: trang xuất hiện CAPTCHA hoặc giới hạn truy cập; dừng gửi form.");
        }
        if (!SubmissionGuard.TryReserve(Settings.MaxFormSubmissions))
            Assert.Inconclusive("Blocked: đã đạt giới hạn số lần gửi form của lần chạy.");

        AllureApi.Step("Nhập dữ liệu kiểm thử (không ghi mật khẩu)", () => page.EnterCredentials(username, password));
        TestContext.Progress.WriteLine($"Submitting form; username category={(string.IsNullOrEmpty(username) ? "empty" : string.IsNullOrWhiteSpace(username) ? "whitespace" : "fake")}; password category={(string.IsNullOrEmpty(password) ? "empty" : string.IsNullOrWhiteSpace(password) ? "whitespace" : "fake")}");
        AllureApi.Step("Gửi form", page.SubmitForm);
        var result = AllureApi.Step("Chờ phản hồi xác thực hoặc giới hạn truy cập", page.WaitForServerErrorOrGuard);
        if (result == "__GUARD__")
        {
            SubmissionGuard.Block();
            Assert.Inconclusive("Blocked: server hiển thị CAPTCHA hoặc giới hạn truy cập.");
        }
        return result;
    }
}
