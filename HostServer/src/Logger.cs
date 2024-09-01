namespace HostServer;

public static class Logger
{
    public static void Log(string msg)
    {
        Console.WriteLine($"[Host|{DateTime.Now:ddd HH:mm:ss}]: {msg}");
    }
}