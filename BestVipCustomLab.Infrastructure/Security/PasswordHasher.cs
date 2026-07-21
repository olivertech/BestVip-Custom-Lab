namespace BestVipCustomLab.Infrastructure.Security;

internal static class PasswordHasher
{
    private const int WorkFactor = 12;

    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public static PasswordVerificationResult Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return PasswordVerificationResult.Failed;
        }

        if (passwordHash.StartsWith("$2", StringComparison.Ordinal))
        {
            var matches = BCrypt.Net.BCrypt.Verify(password, passwordHash);
            if (!matches)
            {
                return PasswordVerificationResult.Failed;
            }

            return BCrypt.Net.BCrypt.PasswordNeedsRehash(passwordHash, WorkFactor)
                ? PasswordVerificationResult.SuccessRehashNeeded
                : PasswordVerificationResult.Success;
        }

        return LegacyPasswordHasher.Verify(password, passwordHash)
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Failed;
    }
}

internal enum PasswordVerificationResult
{
    Failed = 0,
    Success = 1,
    SuccessRehashNeeded = 2
}
