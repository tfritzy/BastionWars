namespace MatchmakingServer;

public static class Logger
{
    public static void Log(string msg)
    {
        Console.WriteLine($"[Match|{DateTime.Now:ddd HH:mm:ss}]: {msg}");
    }
}