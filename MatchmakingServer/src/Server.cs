using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Cache;
using System.Net.WebSockets;
using System.Security.Principal;
using DotNetEnv;
using Google.Protobuf;
using Schema;

namespace MatchmakingServer;

public class Server
{
    private HashSet<string> hostIpAllowlist = new();
    private string url;
    private const int interval = 1000 / 15;

    public Server()
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("Missing HOSTED_ADDRESS in env file");
        string rawAllowlist = Environment.GetEnvironmentVariable("ALLOWLISTED_HOST_IPS")
            ?? throw new Exception("Missing ALLOWLISTED_HOST_IPS in env file");
        hostIpAllowlist = new HashSet<string>(rawAllowlist.Split(","));
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
        await AddConnection(
            context,
            // TODO: Specify they are trying to be a host.
            hostIpAllowlist.Contains(context.Request.RemoteEndPoint.Address.ToString())
        );
    }

    private async Task AddConnection(HttpListenerContext context, bool isHost)
    {
        WebSocketContext? wsContext = null;

        try
        {
            wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
            Console.WriteLine($"Host WebSocket connection established at {context.Request.Url}");
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
            Console.WriteLine("Exception: " + e.Message);
            return;
        }

        var user = wsContext.User;
        WebSocket webSocket = wsContext.WebSocket;

        if (isHost)
        {
            _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleMsgFromHostServer(wsContext, ms)));
        }
        else
        {
            _ = Task.Run(() => ListenLoop(webSocket, async (ms) => await HandleMsgFromPlayer(wsContext, ms)));
        }

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
        }
    }

    private async Task HandleMsgFromPlayer(WebSocketContext context, MemoryStream ms)
    {
        Oneof_PlayerToMatchmaker request = Oneof_PlayerToMatchmaker.Parser.ParseFrom(ms);
        switch (request.MsgCase)
        {
            case Oneof_PlayerToMatchmaker.MsgOneofCase.SearchForGame:
                await SendMessageToHostServer(
                    new Oneof_MatchmakerToHostServer
                    {
                        PlacePlayerInGame = new PlacePlayerInGame()
                        {
                            PlayerId = context.User?.Identity?.Name,
                        }
                    },
                    context.WebSocket);
                break;
            default:
                Console.WriteLine("Invalid type: " + request.MsgCase);
                break;

        }
    }

    private async Task HandleMsgFromHostServer(WebSocketContext context, MemoryStream ms)
    {
        Oneof_HostServerToMatchmaker request = Oneof_HostServerToMatchmaker.Parser.ParseFrom(ms);
        Console.WriteLine("A host said something: " + request.ToString());

        switch (request.MsgCase)
        {
            case Oneof_HostServerToMatchmaker.MsgOneofCase.GameFoundForPlayer:
                await SendMessageToPlayer(
                    new Oneof_MatchMakerToPlayer
                    {
                        FoundGame = request.GameFoundForPlayer
                    },
                    context.WebSocket
                );
                break;
            default:
                Console.WriteLine("Invalid message type from host: " + request.MsgCase);
                break;
        }
    }


    private async Task SendMessageToPlayer(Oneof_MatchMakerToPlayer message, WebSocket webSocket)
    {
        Console.WriteLine($"Matchmaker sending message of type {message.MsgCase} to player");
        byte[] data = message.ToByteArray();
        await webSocket.SendAsync(
            new ArraySegment<byte>(data, 0, data.Length),
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
    }

    private async Task SendMessageToHostServer(Oneof_MatchmakerToHostServer message, WebSocket webSocket)
    {
        Console.WriteLine($"Matchmaker sending message of type {message.MsgCase} to host server");
        byte[] data = message.ToByteArray();
        await webSocket.SendAsync(
            new ArraySegment<byte>(data, 0, data.Length),
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
    }
}