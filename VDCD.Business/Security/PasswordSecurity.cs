using System.Security.Cryptography;

namespace VDCD.Business.Security;

public static class PasswordSecurity
{
    private const string Prefix = "PBKDF2";
    private const int Iterations = 100000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public static bool VerifyPassword(string? storedHash, string? providedPassword)
    {
        if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrEmpty(providedPassword))
            return false;

        if (!storedHash.StartsWith($"{Prefix}$", StringComparison.Ordinal))
            return string.Equals(storedHash, providedPassword, StringComparison.Ordinal);

        var parts = storedHash.Split('$');
        if (parts.Length != 4)
            return false;

        if (!int.TryParse(parts[1], out var iterations))
            return false;

        byte[] salt;
        byte[] expectedKey;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedKey = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(expectedKey, actualKey);
    }
}
