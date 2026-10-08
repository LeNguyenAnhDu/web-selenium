namespace UTC.LoginTests.Helpers;

public static class TestCaseIds
{
    public static string FromTestName(string name)
    {
        if (name.StartsWith("TC-LOGIN-", StringComparison.Ordinal) || name.StartsWith("SEC-LOGIN-", StringComparison.Ordinal))
            return name.Split('_')[0];
        return name switch
        {
            "FakeAccountShowsAuthenticationError" => "TC-LOGIN-004",
            "WhitespaceFieldsShowAuthenticationError" => "TC-LOGIN-005",
            "PasswordInputIsMasked" => "TC-LOGIN-006",
            _ => "UNMAPPED"
        };
    }
}
