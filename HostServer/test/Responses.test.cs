using System.Net.WebSockets;
using Google.Protobuf;
using Helpers;
using Moq;
using Schema;
using Server;

namespace HostServerTest;

[TestClass]
public class Responses
{

    private IWebSocketClient buildWS(
        List<ArraySegment<byte>> sentMessages)
    {
        var mockWebSocketClient = new Mock<IWebSocketClient>();

        mockWebSocketClient
            .Setup(client => client.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockWebSocketClient
            .Setup(client => client.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<ArraySegment<byte>, WebSocketMessageType, bool, CancellationToken>((buffer, messageType, endOfMessage, cancellationToken) => sentMessages.Add(buffer))
            .Returns(Task.CompletedTask);

        var receiveTaskCompletionSource = new TaskCompletionSource<WebSocketReceiveResult>();
        mockWebSocketClient
            .Setup(client => client.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Returns(receiveTaskCompletionSource.Task);

        return mockWebSocketClient.Object;
    }

    [TestMethod]
    public void StandsUpGame()
    {
        List<ArraySegment<byte>> sentMessages = [];
        var ws = buildWS(sentMessages);
        Host server = new(ws);

        Assert.AreEqual(0, sentMessages.Count);
        server.ConnectWithMatchmakingServer();
        var createGame = new Schema.OneofMatchmakingUpdate
        {
            CreateGame = new Schema.CreateGame
            {
                GameId = "game_001",
                Settings = new Schema.GameSettings
                {
                    GenerationMode = Schema.GenerationMode.AutoAccrue,
                    Map = TestHelpers.TestMaps.ThreeByThree,
                }
            }
        };
        server.HandleMessage(createGame).Wait();
        Assert.AreEqual(1, sentMessages.Count);
        OneofMatchmakingRequest req = OneofMatchmakingRequest.Parser.ParseFrom(sentMessages.FirstOrDefault());
        Assert.AreEqual(OneofMatchmakingRequest.RequestOneofCase.GameReady, req.RequestCase);
        Assert.AreEqual("game_001", req.GameReady.GameId);
    }
}