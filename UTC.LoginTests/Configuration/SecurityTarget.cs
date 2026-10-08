using NUnit.Framework;

namespace UTC.LoginTests.Configuration;

public static class SecurityTarget
{
    public static string RequireStagingUrl()
    {
        if (!bool.TryParse(Environment.GetEnvironmentVariable("UTC_SECURITY_TESTS_ENABLED"), out var enabled) || !enabled)
            Assert.Inconclusive("Blocked: security tests chỉ chạy khi UTC_SECURITY_TESTS_ENABLED=true và có staging URL được phép kiểm thử.");

        var raw = Environment.GetEnvironmentVariable("UTC_SECURITY_URL");
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var target))
            Assert.Inconclusive("Blocked: chưa cấu hình UTC_SECURITY_URL hợp lệ cho môi trường staging.");

        if (target!.Host.Equals("vanphongdientu.utc.edu.vn", StringComparison.OrdinalIgnoreCase))
            Assert.Inconclusive("Blocked: không chạy SQL injection probe trên trang UTC production.");

        if (target.Scheme != Uri.UriSchemeHttps && !(target.Scheme == Uri.UriSchemeHttp && target.IsLoopback))
            Assert.Inconclusive("Blocked: staging cần HTTPS; HTTP chỉ được dùng với localhost.");

        return target.AbsoluteUri;
    }
}
