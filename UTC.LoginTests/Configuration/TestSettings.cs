using System.Text.Json;

namespace UTC.LoginTests.Configuration;

public sealed record TestSettings(string Url, int TimeoutSeconds, bool Headless, int MaxFormSubmissions)
{
    public static TestSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        return new TestSettings(
            Environment.GetEnvironmentVariable("UTC_LOGIN_URL") ?? root.GetProperty("Url").GetString()!,
            int.TryParse(Environment.GetEnvironmentVariable("UTC_TIMEOUT_SECONDS"), out var timeout) ? timeout : root.GetProperty("TimeoutSeconds").GetInt32(),
            bool.TryParse(Environment.GetEnvironmentVariable("UTC_HEADLESS"), out var headless) ? headless : root.GetProperty("Headless").GetBoolean(),
            int.TryParse(Environment.GetEnvironmentVariable("UTC_MAX_SUBMISSIONS"), out var max) ? max : root.GetProperty("MaxFormSubmissions").GetInt32());
    }
}
