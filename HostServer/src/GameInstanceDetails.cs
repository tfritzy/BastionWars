using System.Diagnostics;
using Helpers;

namespace HostServer;

public struct GameInstanceDetails
{
    public Process Process;
    public WebSocketClient WebSocket;
    public int Port;
    public string Id;
}