namespace UTC.LoginTests.TestData;

public static class LoginCases
{
    public const string FakeUsername = "utc_qa_nonexistent_20261008";
    public const string FakePassword = "TestOnly!2026";

    public static (string Username, string Password, string ExpectedError) ForId(string id) => id switch
    {
        "TC-LOGIN-001" => ("", "", "Bạn chưa nhập tên đăng nhập"),
        "TC-LOGIN-002" => (FakeUsername, "", "Bạn chưa nhập mật khẩu"),
        "TC-LOGIN-003" => ("", FakePassword, "Bạn chưa nhập tên đăng nhập"),
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };
}
