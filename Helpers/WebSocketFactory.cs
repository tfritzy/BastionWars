namespace Helpers;

public abstract class WebSocketFactory
{
    public abstract IWebSocketClient Build();
}