using System.Text;

namespace Helpers;

public static class IdGenerator
{
    private static readonly Random random = new Random();
    public const string GamePrefix = "game";
    public const string PlayerPrefix = "plyr";
    public const string AuthTokenPrefix = "auth";

    public static string GenerateId(string prefix)
    {
        var randomString = GenerateRandomString(8);
        return $"{prefix}_{randomString}";
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }
        return stringBuilder.ToString();
    }

    public static string GenerateGameId()
    {
        return GenerateId(GamePrefix);
    }

    public static string GeneratePlayerId()
    {
        return GenerateId(PlayerPrefix);
    }

    public static string GenerateAuthToken()
    {
        return GenerateId(AuthTokenPrefix);
    }
}