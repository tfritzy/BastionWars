namespace Tests;

public static class Maps
{
    static string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");
    public static string TenByFive => File.ReadAllText(basePath + "/TenByFive.txt");
    public static string ThirtyByTwenty => File.ReadAllText(basePath + "/ThirtyByTwenty.txt");
}