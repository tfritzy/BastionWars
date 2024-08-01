using System.Net;
using System.Net.WebSockets;
using System.Text;
using DotNetEnv;

namespace HostServer;

public class GameServer
{
    private string matchmakingServerAddress;
    private string hostedAddress;

    public GameServer()
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

        await ws.ConnectAsync(new Uri(matchmakingServerAddress), CancellationToken.None);
        Console.WriteLine("Connected!");

        // Send a message
        string message = "Hello, WebSocket!";
        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Sent: {message}");

        // Receive a message
        byte[] buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received: {receivedMessage}");

        // Close the WebSocket connection
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        Console.WriteLine("Connection closed.");
    }
}