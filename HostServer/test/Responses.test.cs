using HostServer;
using Moq;
using Schema;
using Moq.Contrib.HttpClient;
using System.Net;
using Google.Protobuf;

namespace HostServerTest;

[TestClass]
public class Responses
{
    Server server;

    [TestCleanup]
    public void After()
    {
        server.TearDown();
    }

    [TestMethod]
    public void RegistersWithMatchmakingServer()
    {
        server = new Server(new HttpClient());
        var placePlayer = new PlacePlayerInGame
        {
            PlayerId = "plyr_001"
        };
        var deets = server.HandlePlacePlayer(placePlayer).Result;

        Assert.AreEqual(200, deets.StatusCode);
        Assert.AreEqual(server.Games[0].Id, deets.Body!.GameId);
        Assert.AreEqual("plyr_001", deets.Body!.PlayerId);
        Assert.AreEqual(server.Games[0].Port, deets.Body!.Port);
    }

    [TestMethod]
    public void HandlesPlacePlayer()
    {
        Registered registered = new()
        {
            Port = "1250"
        };
        bool called = false;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.OK, registered.ToByteArray(), "application/x-protobuf")
            .Callback(() => called = true);
        var httpClient = handler.CreateClient();

        server = new Server(httpClient);
        Assert.IsFalse(called);
        server.Setup().Wait();
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void ThrowsIfMatchmakerDead()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.BadGateway);
        var httpClient = handler.CreateClient();

        server = new Server(httpClient);
        Assert.ThrowsException<AggregateException>(() => server.Setup().Wait());
    }
}