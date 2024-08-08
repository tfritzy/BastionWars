namespace Helpers;

public static class Maps
{
    static string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");
    public static string Map => File.ReadAllText(basePath + "/map.txt");
}