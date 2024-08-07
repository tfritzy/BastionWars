using System.Net.WebSockets;

namespace Helpers;

public class WebSocketClient : IWebSocketClient
{
    private ClientWebSocket ws = new();

    public async Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    {
        await ws.CloseAsync(closeStatus, statusDescription, cancellationToken);
    }

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        await ws.ConnectAsync(uri, cancellationToken);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        return await ws.ReceiveAsync(buffer, cancellationToken);
    }

    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
    {
        await ws.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }
}