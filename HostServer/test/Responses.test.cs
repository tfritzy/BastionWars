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
        List<ArraySegment<byte>> sentMessages = [];
        Host server = new(new TestWebSocketFactory());

        Assert.AreEqual(0, sentMessages.Count);
        server.Setup().Wait();
        var placePlayer = new OneofMatchmakingUpdate
        {
            PlacePlayerInGame = new PlacePlayerInGame
            {
                PlayerId = "plyr_0001"
            }
        };
        server.HandleMessage(placePlayer).Wait();
        Assert.AreEqual(1, sentMessages.Count);
        OneofMatchmakingRequest req = OneofMatchmakingRequest.Parser.ParseFrom(sentMessages.FirstOrDefault());
        Assert.AreEqual(OneofMatchmakingRequest.RequestOneofCase.GameFoundForPlayer, req.RequestCase);
        Assert.AreEqual("plyr_0001", placePlayer.PlacePlayerInGame.PlayerId);
    }
}