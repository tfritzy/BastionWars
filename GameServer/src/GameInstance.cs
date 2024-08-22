using System.Net;
using System.Net.WebSockets;
using DotNetEnv;
using Schema;

namespace GameServer;

public class GameInstance
{
    public string Id { get; private set; }
    public Schema.GameSettings GameSettings { get; private set; }

    private string url;

    public GameInstance(string id, string port, GameSettings gameSettings)
    {
        Id = id;
        GameSettings = gameSettings;
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? "game_server.env.production" : "game_server.env";
        Env.Load(envFile);

        url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("Missing HOSTED_ADDRESS in env file");
        url = $"{url}:{port}/";
        Console.WriteLine("url: " + url);
    }

    public void StartGame()
    {
        StartAcceptingConnections();
        Console.WriteLine($"Starting game {Id}");
        while (true) ;
    }

    public async void StartAcceptingConnections()
    {
        HttpListener httpListener = new();

        if (string.IsNullOrEmpty(url))
        {
            throw new Exception("mmm no url to listen on");
        }

        httpListener.Prefixes.Add(url);
        httpListener.Start();
        Console.WriteLine("GameServer instance listening on " + url);

        try
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var _ = Task.Run(() => AddConnection(context));
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

    private async Task AddConnection(HttpListenerContext context)
    {
        WebSocketContext? webSocketContext = null;

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            Console.WriteLine($"Host WebSocket connection established at {context.Request.Url}");
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
            Console.WriteLine("Exception: " + e.Message);
            return;
        }

        WebSocket webSocket = webSocketContext.WebSocket;
        _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleMsgFromPlayer(webSocket, ms)));
    }


    private async void ListenLoop(WebSocket webSocket, Action<MemoryStream> handleRequest)
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
                    using (MemoryStream? ms = new MemoryStream(receiveBuffer, 0, messageLength))
                    {
                        handleRequest(ms);
                    }
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket connection closed by client.");
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        CancellationToken.None);
                }
                else
                {
                    Console.WriteLine("Received unparseable message of type " + receiveResult.MessageType);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in listen loop: " + e.Message);
            _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleMsgFromPlayer(webSocket, ms)));
        }
    }

    private async Task HandleMsgFromPlayer(WebSocket webSocket, MemoryStream ms)
    {
        Oneof_HostServerToGameServer request = Oneof_HostServerToGameServer.Parser.ParseFrom(ms);
        Console.WriteLine("A host said something: " + request.ToString());

        switch (request.MsgCase)
        {
            default:
                Console.WriteLine("GameServer got invalid message type from player: " + request.MsgCase);
                break;
        }
    }
}