namespace GameServer;

public static class Logger
{
    public static void Log(string msg)
    {
        Console.WriteLine($"[Game|{DateTime.Now:ddd HH:mm:ss}]: {msg}");
    }
}