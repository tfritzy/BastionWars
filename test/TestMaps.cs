namespace Tests;

public static class TestMaps
{
    static string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");
    public static string ThreeByThree => File.ReadAllText(basePath + "/FourByFour.txt");
    public static string TenByFive => File.ReadAllText(basePath + "/TenByFive.txt");
    public static string ThirtyByTwenty => File.ReadAllText(basePath + "/ThirtyByTwenty.txt");
}