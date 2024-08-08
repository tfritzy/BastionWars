using System.Net.WebSockets;
using Helpers;
using Moq;

namespace TestHelpers;

public class TestWebSocketFactory : WebSocketFactory
{
    public override IWebSocketClient Build()
    {
        var mockWebSocketClient = new Mock<IWebSocketClient>();
        List<ArraySegment<byte>> sentMessages = [];

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

        mockWebSocketClient
            .Setup(client => client.TestOnly_GetSentMessages())
            .Returns(sentMessages);

        return mockWebSocketClient.Object;
    }
}