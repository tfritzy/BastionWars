using System.Net;
using System.Net.Cache;
using System.Net.WebSockets;
using DotNetEnv;
using Schema;

namespace MatchmakingServer;

public class Server
{
    public Dictionary<string, WebSocket> ConnectedPlayers { get; set; }

    private const int interval = 1000 / 15;

    public Server()
    {
        ConnectedPlayers = new Dictionary<string, WebSocket>();
    }

    public async void Update()
    {
        var nextTick = DateTime.Now.AddMilliseconds(interval);
        int delay = (int)(nextTick - DateTime.Now).TotalMilliseconds;
        if (delay > 0)
            Thread.Sleep(delay);
    }

    public async void StartAcceptingConnections()
    {
        HttpListener httpListener = new();
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        string? url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS");
        if (String.IsNullOrEmpty(url))
        {
            throw new Exception("HOSTED_ADDRESS environment variable not set.");
        }

        httpListener.Prefixes.Add(url);
        httpListener.Start();
        Console.WriteLine("Listening on " + url);

        try
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var _ = Task.Run(() => AcceptConnection(context));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to accept connection: " + e.Message);

            _ = Task.Run(() => StartAcceptingConnections());
        }
    }


    public async Task AcceptConnection(HttpListenerContext context)
    {
        WebSocketContext? webSocketContext = null;
        var id = context.Request.QueryString["id"];
        if (id == null)
        {
            Console.WriteLine("Client did not specify an id. Kicking them.");
            context.Response.StatusCode = 400;
            context.Response.Close();
            return;
        }

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            ConnectedPlayers[id] = webSocketContext.WebSocket;
            Console.WriteLine($"WebSocket connection established at {context.Request.Url}");
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
            Console.WriteLine("Exception: " + e.Message);
            return;
        }

        WebSocket webSocket = webSocketContext.WebSocket;
        _ = Task.Run(() => ListenLoop(webSocket, id));
    }

    private async void ListenLoop(WebSocket webSocket, string token)
    {
        try
        {
            byte[] receiveBuffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                int messageLength = receiveResult.Count;

                if (receiveResult.MessageType == WebSocketMessageType.Binary)
                {
                    using (var ms = new MemoryStream(receiveBuffer, 0, messageLength))
                    {
                        OneofRequest request = OneofRequest.Parser.ParseFrom(ms);
                        Console.WriteLine(request.ToString());
                    }
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket connection closed by client.");
                    ConnectedPlayers.Remove(token);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in listen loop: " + e.Message);
        }
    }
}