using Server;

namespace HostServerTest;

[TestClass]
public class Responses
{
    [TestMethod]
    public void StandsUpGame()
    {
        Dictionary<string, List<byte[]>> sentMessages = new();
        Host server = new();
    }
}