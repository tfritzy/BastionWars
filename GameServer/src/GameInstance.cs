using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
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
    private readonly Dictionary<string, WebSocketContext> connections = [];
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
            double time = stopwatch.ElapsedMilliseconds / 1000.0;
            game.Update(time - lastTime);
            lastTime = time;
            await DrainPendingMessages();
        }
    }

    private async Task DrainPendingMessages()
    {
        foreach (Player player in game.Players.Values)
        {
            if (connections.TryGetValue(player.Id, out WebSocketContext? context))
            {
                foreach (Oneof_GameServerToPlayer msg in player.MessageQueue)
                {
                    Logger.Log($"Sending a message to player {msg.RecipientId}");
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

                player.MessageQueue.Clear();
            }
        }
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
        string? playerId = context.Request.QueryString["playerId"];
        string? authToken = context.Request.QueryString["authToken"];

        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(authToken))
        {
            context.Response.StatusCode = 400;
            context.Response.Close();
            Logger.Log("Player connected without auth token or player id");
            return;
        }

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            connections.Add(playerId, webSocketContext);
            game.JoinGame(new Player("TODO: Player name", playerId));
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
        _ = Task.Run(() => ListenLoop(webSocket, playerId, (ms) => HandleMsgFromPlayer(ms)));
    }


    private async void ListenLoop(WebSocket webSocket, string playerId, Action<MemoryStream> handleRequest)
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
                    connections.Remove(playerId);
                    game.DisconnectPlayer(playerId);
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
            _ = Task.Run(() => ListenLoop(webSocket, playerId, (ms) => HandleMsgFromPlayer(ms)));
        }
    }

    private void HandleMsgFromPlayer(MemoryStream ms)
    {
        try
        {
            Oneof_PlayerToGameServer request = Oneof_PlayerToGameServer.Parser.ParseFrom(ms);
            game.HandleCommand(request);
        }
        catch (Exception e)
        {
            Logger.Log("Was unable to parse or handle message from player. " + e);
        }
    }
}