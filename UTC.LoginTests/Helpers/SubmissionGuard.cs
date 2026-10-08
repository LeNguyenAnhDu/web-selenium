using System.Threading;

namespace UTC.LoginTests.Helpers;

public static class SubmissionGuard
{
    private static int _count;
    private static int _blocked;
    public static bool IsBlocked => Volatile.Read(ref _blocked) != 0;
    public static bool TryReserve(int maximum) => !IsBlocked && Interlocked.Increment(ref _count) <= maximum;
    public static void Block() => Interlocked.Exchange(ref _blocked, 1);
}
