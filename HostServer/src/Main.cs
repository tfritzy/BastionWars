using System.Net.WebSockets;
using Helpers;
using Server;

WebSocketClient ws = new();
Host host = new(ws);
await host.ConnectWithMatchmakingServer();
await host.ListenLoop();