using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using DotNetEnv;
using Google.Protobuf;
using Helpers;
using KeepLordWarriors;
using Schema;

namespace GameServer;

public class GameInstance
{
    public string Id { get; private set; }
    public Schema.GameSettings GameSettings { get; private set; }
    private string url;
    private HttpListener httpListener;
    private Game game;
    private Dictionary<string, WebSocketContext> connections = new();
    const int ChunkSize = 4096;

    public GameInstance(string id, string port, GameSettings gameSettings)
    {
        EnvHelpers.Init();
        Id = id;
        GameSettings = gameSettings;
        game = new Game(gameSettings);
        url = $"http://[::1]:{port}/";
        httpListener = new();
    }

    public async void StartGame()
    {
        StartAcceptingConnections();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        double lastTime = 0;
        while (true)
        {
            game.Update(stopwatch.ElapsedMilliseconds / 1000.0 - lastTime);
            lastTime = stopwatch.Elapsed.TotalSeconds;
            await DrainOutbox();
        }
    }

    private async Task DrainOutbox()
    {
        foreach (Oneof_GameServerToPlayer msg in game.Outbox)
        {
            if (connections.TryGetValue(msg.RecipientId, out WebSocketContext? context))
            {
                byte[] messageBytes = msg.ToByteArray();
                for (int i = 0; i < messageBytes.Length; i += ChunkSize)
                {
                    int remainingBytes = Math.Min(ChunkSize, messageBytes.Length - i);
                    bool isLastChunk = (i + remainingBytes) >= messageBytes.Length;

                    await context.WebSocket.SendAsync(
                        new ArraySegment<byte>(messageBytes, i, remainingBytes),
                        WebSocketMessageType.Binary,
                        isLastChunk,
                        CancellationToken.None);
                }
            }
        }

        game.Outbox.Clear();
    }

    private void StartAcceptingConnections()
    {
        httpListener.Prefixes.Add(url);
        httpListener.Start();
        _ = Task.Run(() => Listen());
        Logger.Log("GameServer instance listening on " + url);
    }

    public async Task Listen()
    {
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
            Logger.Log("Failed to accept connection: " + e.Message);
            await Listen();
        }
    }

    private async Task AddConnection(HttpListenerContext context)
    {
        WebSocketContext? webSocketContext = null;

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            Logger.Log($"Host WebSocket connection established at {context.Request.Url}");
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
            Logger.Log("Exception: " + e.Message);
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
                else if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    using (MemoryStream? ms = new MemoryStream(receiveBuffer, 0, messageLength))
                    {
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            string textMessage = await reader.ReadToEndAsync();
                            Logger.Log("Got unusable text message. It says: " + textMessage);
                        }
                    }
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    Logger.Log("WebSocket connection closed by client.");
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        CancellationToken.None);
                }
                else
                {
                    Logger.Log("Received unparseable message of type " + receiveResult.MessageType);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Log("Exception in listen loop: " + e.Message);
            _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleMsgFromPlayer(webSocket, ms)));
        }
    }

    private async Task HandleMsgFromPlayer(WebSocket webSocket, MemoryStream ms)
    {
        Oneof_HostServerToGameServer request = Oneof_HostServerToGameServer.Parser.ParseFrom(ms);
        Logger.Log("A host said something: " + request.ToString());

        switch (request.MsgCase)
        {
            default:
                Logger.Log("GameServer got invalid message type from player: " + request.MsgCase);
                break;
        }
    }
}