namespace Helpers;

public class LiveWebSocketFactory : WebSocketFactory
{
    public override IWebSocketClient Build()
    {
        return new WebSocketClient();
    }
}