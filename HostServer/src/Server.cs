using System.Diagnostics;
using System.Net.WebSockets;
using DotNetEnv;
using Google.Protobuf;
using Helpers;
using Schema;

namespace Server;

public class Host
{
    private string matchmakingServerAddress;
    private string hostedAddress;
    private IWebSocketClient ws;

    public Host(IWebSocketClient ws)
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        matchmakingServerAddress = Environment.GetEnvironmentVariable("MATCHMAKING_SERVER_ADDRESS")
            ?? throw new Exception("MATCHMAKING_SERVER_ADDRESS environment variable not set.");
        hostedAddress = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("HOSTED_ADDRESS variable missing");

        this.ws = ws;
    }

    public async Task ConnectWithMatchmakingServer()
    {
        await ws.ConnectAsync(new Uri(matchmakingServerAddress + $"?id=host_asdf"), CancellationToken.None);
    }

    public async Task ListenLoop()
    {
        byte[] buffer = new byte[1024];
        var result = await ws.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);
        int messageLength = result.Count;
        if (result.MessageType == WebSocketMessageType.Binary)
        {
            using (MemoryStream? ms = new MemoryStream(buffer, 0, messageLength))
            {
                try
                {
                    OneofMatchmakingUpdate message = OneofMatchmakingUpdate.Parser.ParseFrom(ms);
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
            await ws.CloseAsync(
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

    public async Task HandleMessage(OneofMatchmakingUpdate message)
    {
        switch (message.UpdateCase)
        {
            case OneofMatchmakingUpdate.UpdateOneofCase.CreateGame:
                StartGameInstance(message.CreateGame);
                await SendMessage(
                    new OneofMatchmakingRequest
                    {
                        GameReady = new GameReady
                        {
                            GameId = message.CreateGame.GameId,
                        }
                    });
                break;
            default:
                Console.WriteLine("Uknown message type: " + message.UpdateCase);
                break;

        }
    }

    private async Task SendMessage(OneofMatchmakingRequest req)
    {
        byte[] bytesToSend = req.ToByteArray();
        await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private static void StartGameInstance(CreateGame createGame)
    {
        string gameSettings = Convert.ToBase64String(createGame.Settings.ToByteArray());

        string currentDirectory = Directory.GetCurrentDirectory();
        Console.WriteLine("Current working directory: " + currentDirectory);

        Process process = new();
        process.StartInfo.FileName = $"GameServer.exe";
        process.StartInfo.Arguments = $"{createGame.GameId} {gameSettings}";
        process.StartInfo.UseShellExecute = true;

        process.Start();
        process.WaitForExit();
    }
}
