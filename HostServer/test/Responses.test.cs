using System.Net.WebSockets;
using Google.Protobuf;
using Helpers;
using HostServer;
using Moq;
using Schema;
using TestHelpers;

namespace HostServerTest;

[TestClass]
public class Responses
{
    [TestMethod]
    public void StandsUpGame()
    {
        var server = new Host(new TestWebSocketFactory());
        Assert.AreEqual(0, server.Games.Count);
        server.Setup().Wait();
        Assert.AreEqual(1, server.Games.Count);
    }

    [TestMethod]
    public void PlacesPlayersInGame()
    {
        var server = new Host(new TestWebSocketFactory());
        server.Setup().Wait();

        server.HandleMessage(
            new Oneof_MatchmakerToHostServer
            {
                PlacePlayerInGame = new PlacePlayerInGame
                {
                    PlayerId = "plyr_001"
                }
            }
        ).Wait();

        Assert.Fail("Check that the host server sent a message to the matchmaking server");
    }
}