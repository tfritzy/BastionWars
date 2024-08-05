using System.Diagnostics;
using System.Net.WebSockets;
using DotNetEnv;
using Google.Protobuf;
using Schema;

namespace Server;

public class Host
{
    private string matchmakingServerAddress;
    private string hostedAddress;

    public delegate void MessageHandler(object sender, byte[] bytes);
    public event MessageHandler OnMessageSent;

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
        ClientWebSocket ws = new();
        await ws.ConnectAsync(new Uri(matchmakingServerAddress + $"?id=host_asdf"), CancellationToken.None);
        await ListenLoop(ws);
    }

    private async Task ListenLoop(WebSocket ws)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            var result = await ws.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);
            int messageLength = result.Count;
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                using (MemoryStream? ms = new MemoryStream(buffer, 0, messageLength))
                {
                    await HandleMessage(ws, ms);
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
        }
    }

    private async Task HandleMessage(WebSocket ws, MemoryStream ms)
    {
        OneofMatchmakingUpdate message = OneofMatchmakingUpdate.Parser.ParseFrom(ms);
        Console.WriteLine($"Received message from host of type {message.UpdateCase}");
        switch (message.UpdateCase)
        {
            case OneofMatchmakingUpdate.UpdateOneofCase.CreateGame:
                StartGameInstance();
                await SendMessage(
                    new OneofMatchmakingRequest
                    {
                        GameReady = new GameReady
                        {
                            GameId = message.CreateGame.GameId,
                        }
                    },
                    ws);
                break;
            default:
                Console.WriteLine("Uknown message type: " + message.UpdateCase);
                break;

        }
    }

    private async Task SendMessage(OneofMatchmakingRequest req, WebSocket ws)
    {
        byte[] bytesToSend = req.ToByteArray();
        await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
        OnMessageSent.Invoke(this, bytesToSend);
    }

    private static void StartGameInstance(CreateGame createGame)
    {
        string gameSettings = Convert.ToBase64String(createGame.Settings.ToByteArray());

        try
        {
            Process process = new();
            process.StartInfo.FileName = $"{createGame.GameId}.exe";
            process.StartInfo.Arguments = $"{createGame.GameId} {gameSettings}";
            process.StartInfo.UseShellExecute = true;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    private static void StartGameInstance()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
        string buildConfig = "Debug";
#else
            string buildConfig = "Release";
#endif

        // Construct the relative path to the other project's executable
        string relativePath = $@"..\..\OtherProject\bin\{buildConfig}\OtherProject.exe";
        string exePath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

        // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = "", // Add any arguments here if needed
            UseShellExecute = false, // Set to true if you want to use the OS shell to start the process
            RedirectStandardOutput = false, // Set to true if you want to read the standard output
            RedirectStandardError = false, // Set to true if you want to read the standard error
            CreateNoWindow = false // Set to true if you don't want to create a window
        };

        try
        {
            // Start the process
            using (Process process = Process.Start(startInfo))
            {
                Console.WriteLine("Process started successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start process: {ex.Message}");
        }
    }
}
