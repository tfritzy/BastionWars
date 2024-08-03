using System.Net;
using System.Net.WebSockets;
using System.Text;
using DotNetEnv;
using Google.Protobuf;
using Schema;

namespace HostServer;

public class Host
{
    private string matchmakingServerAddress;
    private string hostedAddress;

    public Host()
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        matchmakingServerAddress = Environment.GetEnvironmentVariable("MATCHMAKING_SERVER_ADDRESS")
            ?? throw new Exception("MATCHMAKING_SERVER_ADDRESS environment variable not set.");
        hostedAddress = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("HOSTED_ADDRESS variable missing");
    }

    public async Task ConnectWithMatchmakingServer()
    {
        ClientWebSocket ws = new ClientWebSocket();

        await ws.ConnectAsync(new Uri(matchmakingServerAddress + $"?id=host_asdf"), CancellationToken.None);
        Console.WriteLine("Connected!");

        await SendMessage(
            new Schema.OneofMatchmakingRequest
            {
                HostIntroduction = new Schema.HostIntroduction
                {
                    FavoriteColor = "green",
                }
            },
            ws);

        await ListenLoop(ws);
    }

    private async Task ListenLoop(WebSocket ws)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            int messageLength = result.Count;
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                using (MemoryStream? ms = new MemoryStream(buffer, 0, messageLength))
                {
                    await HandleMessage(ms);
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("WebSocket connection closed by matchmaking server.");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                Console.WriteLine("Received unparseable message of type " + result.MessageType);
            }
        }
    }

    private async Task HandleMessage(MemoryStream ms)
    {
        OneofMatchmakingUpdate message = OneofMatchmakingUpdate.Parser.ParseFrom(ms);
        Console.WriteLine($"Received message from host of type {message.UpdateCase}");
        switch (message.UpdateCase)
        {
            case OneofMatchmakingUpdate.UpdateOneofCase.HostHello:
                Console.WriteLine("The host knows my favorite color is: " + message.HostHello.FavoriteColor);
                break;
            default:
                Console.WriteLine("Uknown message type: " + message.UpdateCase);
                break;

        }
    }

    private async Task SendMessage(Schema.OneofMatchmakingRequest req, WebSocket ws)
    {
        byte[] bytesToSend = req.ToByteArray();
        await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}