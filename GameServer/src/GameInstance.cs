using System.Net;
using System.Net.WebSockets;
using DotNetEnv;
using Schema;

namespace GameServer;

class GameInstance
{
    public string Id { get; private set; }
    public Schema.GameSettings GameSettings { get; private set; }

    private string url;

    public GameInstance(string id, string port, Schema.GameSettings gameSettings)
    {
        Id = id;
        GameSettings = gameSettings;
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("Missing HOSTED_ADDRESS in env file");
        url = $"{url}:{port}/";
    }

    public void StartGame()
    {
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
        Console.WriteLine("Listening on " + url);

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
        _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleHostMessage(webSocket, ms)));
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
            _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleHostMessage(webSocket, ms)));
        }
    }

    private async Task HandleHostMessage(WebSocket webSocket, MemoryStream ms)
    {
        OneofMatchmakingRequest request = OneofMatchmakingRequest.Parser.ParseFrom(ms);
        Console.WriteLine("A host said something: " + request.ToString());

        switch (request.RequestCase)
        {
            default:
                Console.WriteLine("Invalid message type from host: " + request.RequestCase);
                break;
        }
    }
}