using System.Diagnostics;
using System.Net.WebSockets;
using DotNetEnv;
using GameServer;
using Google.Protobuf;
using Helpers;
using Schema;

namespace HostServer;

public class Host
{
    public IWebSocketClient HostWS { get; private set; }
    public readonly List<GameInstanceDetails> Games = [];

    private readonly string matchmakingServerAddress;
    private readonly string hostedAddress;
    private readonly string selfAddress;
    private readonly List<int> availablePorts;

    private const int START_PORT = 1750;
    private const int MAX_GAMES = 10;

    public Host(WebSocketFactory webSocketFactory)
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        matchmakingServerAddress = Environment.GetEnvironmentVariable("MATCHMAKING_SERVER_ADDRESS")
            ?? throw new Exception("MATCHMAKING_SERVER_ADDRESS environment variable not set.");
        hostedAddress = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("HOSTED_ADDRESS variable missing");
        selfAddress = Environment.GetEnvironmentVariable("SELF_ADDRESS")
            ?? throw new Exception("SELF_ADDRESS variable missing");

        HostWS = webSocketFactory.Build();
        availablePorts = GetInitialPorts();
    }

    public async Task Setup()
    {
        Console.WriteLine("Starting up host server");
        await StartGameInstance();
        await HostWS.ConnectAsync(new Uri(matchmakingServerAddress + $"?id=host_asdf"), CancellationToken.None);
    }

    public async Task ListenLoop()
    {
        byte[] buffer = new byte[1024];
        var result = await HostWS.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);
        int messageLength = result.Count;
        if (result.MessageType == WebSocketMessageType.Binary)
        {
            using (MemoryStream? ms = new MemoryStream(buffer, 0, messageLength))
            {
                try
                {
                    Oneof_MatchmakerToHostServer message = Oneof_MatchmakerToHostServer.Parser.ParseFrom(ms);
                    await HandleMessage(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Host server got unparsable message. " + e.ToString());
                }
            }
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("WebSocket connection closed by matchmaking server.");
            await HostWS.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                string.Empty,
                CancellationToken.None);
        }
        else
        {
            Console.WriteLine("Invalid message of type " + result.MessageType);
        }

        await ListenLoop();
    }

    public async Task HandleMessage(Oneof_MatchmakerToHostServer message)
    {
        switch (message.MsgCase)
        {
            case Oneof_MatchmakerToHostServer.MsgOneofCase.PlacePlayerInGame:
                GameInstanceDetails details = GetGameForPlayer(message.PlacePlayerInGame);
                await SendMessage(
                    new Oneof_HostServerToMatchmaker
                    {
                        GameFoundForPlayer = new GameFoundForPlayer
                        {
                            GameId = details.Id,
                            PlayerId = message.PlacePlayerInGame.PlayerId,
                            Address = $"{selfAddress}:{details.Port}"
                        }
                    });
                break;
            default:
                Console.WriteLine("Uknown message type: " + message.MsgCase);
                break;

        }
    }

    private async Task SendMessage(Oneof_HostServerToMatchmaker req)
    {
        byte[] bytesToSend = req.ToByteArray();
        await HostWS.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private async Task StartGameInstance()
    {
        Console.WriteLine("Starting up game instance");
        var settings = new GameSettings
        {
            GenerationMode = GenerationMode.Word,
            Map = ""
        };
        string gameId = IdGenerator.GenerateGameId();
        int port = availablePorts.First();
        availablePorts.RemoveAt(0);

        var inst = new GameInstance(gameId, port, settings);
        Task task = Task.Run(() => inst.StartGame());

        Games.Add(new GameInstanceDetails
        {
            Task = task,
            Port = port,
            Id = gameId
        });
    }


    private GameInstanceDetails GetGameForPlayer(PlacePlayerInGame placePlayer)
    {
        // Eventually this should do stuff like check how many players are in game
        // And how many starting locations are available
        return Games.First();
    }

    private static List<int> GetInitialPorts()
    {
        List<int> ports = new(MAX_GAMES);
        for (int i = START_PORT; i < START_PORT + MAX_GAMES; i++)
        {
            ports.Add(i);
        }

        return ports;
    }
}
