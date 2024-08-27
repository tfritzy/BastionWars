using System.Net;
using System.Net.Http.Headers;
using DotNetEnv;
using GameServer;
using Google.Protobuf;
using Helpers;
using Schema;

namespace HostServer;

public class Server
{
    private readonly Dictionary<string, Func<HttpListenerContext, Task>> _routes = [];
    private readonly HttpClient httpClient;
    public readonly List<GameInstanceDetails> Games = [];
    private readonly List<int> availablePorts;
    private readonly string matchmakingServerAddress;
    private readonly string port;
    private HttpListener httpListener;

    private const int START_PORT = 1750;
    private const int MAX_GAMES = 100;

    public Server(HttpClient client)
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        matchmakingServerAddress = Environment.GetEnvironmentVariable("MATCHMAKING_SERVER_ADDRESS")
            ?? throw new Exception("MATCHMAKING_SERVER_ADDRESS environment variable not set.");
        port = Environment.GetEnvironmentVariable("PORT")
            ?? throw new Exception("Missing PORT in env file");

        httpClient = client;
        availablePorts = GetInitialPorts();

        httpListener = new();
        httpListener.Prefixes.Add($"http://localhost:{port}/");

        _routes.Add("/place-player", async (HttpListenerContext context) =>
        {
            Console.WriteLine("Asked to place player in game");
            var body = await ReadBodyMatchmaker(context);
            if (body == null) return;
            var gameDetails = await HandlePlacePlayer(body);
            var responseBody = new Oneof_HostServerToMatchmaker
            {
                GameAvailableOnPort = gameDetails.Body
            };
            context.Response.StatusCode = gameDetails.StatusCode;
            WriteBodyForMatchmaker(responseBody, context.Response);
        });

        StartGameInstance();
    }

    public async Task Setup()
    {
        httpListener.Start();
        await RegisterWithMatchmakingServer();
    }

    public void TearDown()
    {
        httpListener.Stop();
    }

    public async Task Listen()
    {
        Console.WriteLine("Listening on " + port);

        try
        {
            var context = await httpListener.GetContextAsync();

            Console.WriteLine("Got one");

            context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:7249");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            if (context.Request.HttpMethod == "OPTIONS")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            else
            {
                var path = context.Request.Url?.AbsolutePath.ToLower() ?? "";

                if (_routes.ContainsKey(path))
                {
                    var action = _routes[path];
                    var _ = Task.Run(() => action(context));
                }
                else
                {
                    HandleNotFound(context);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to accept connection: " + e.Message);
        }

        await Listen();
    }


    public async Task<ResponseDetails<GameAvailableOnPort>> HandlePlacePlayer(Oneof_MatchmakerToHostServer request)
    {
        Console.WriteLine($"Asked to place player {request.PlacePlayerInGame.PlayerId} in a game.");

        GameInstanceDetails details = GetGameForPlayer(request.PlacePlayerInGame);
        Console.WriteLine($"Giving them {details.Id} on port {details.Port}");
        return new ResponseDetails<GameAvailableOnPort>
        {
            Body = new GameAvailableOnPort()
            {
                Port = details.Port,
                GameId = details.Id,
                PlayerId = request.PlacePlayerInGame.PlayerId,
            },
            StatusCode = 200,
        };
    }

    private void HandleNotFound(HttpListenerContext context)
    {
        context.Response.StatusCode = 404;
        context.Response.Close();
    }

    private static async Task<Oneof_MatchmakerToHostServer?> ReadBodyMatchmaker(HttpListenerContext context)
    {
        return await ReadBody(context, Oneof_MatchmakerToHostServer.Parser.ParseFrom);
    }

    private static async Task<T?> ReadBody<T>(
        HttpListenerContext context,
        Func<byte[], T> parseMessage)
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await context.Request.InputStream.CopyToAsync(memoryStream);
                byte[] bodyBytes = memoryStream.ToArray();
                return parseMessage(bodyBytes);
            }
        }
        catch
        {
            Console.WriteLine("Could not parse body into expected format");
            context.Response.StatusCode = 400;
            context.Response.Close();
            return default;
        }
    }
    private void WriteBodyForMatchmaker(Oneof_HostServerToMatchmaker message, HttpListenerResponse response)
    {
        response.ContentType = "application/x-protobuf";

        if (message == null)
        {
            return;
        }

        using (var outputStream = response.OutputStream)
        {
            message.WriteTo(outputStream);
        }
    }


    private GameInstanceDetails GetGameForPlayer(PlacePlayerInGame placePlayer)
    {
        // Eventually this should do stuff like check how many players are in game
        // And how many starting locations are available
        return Games.First();
    }


    private void StartGameInstance()
    {
        Console.WriteLine("Starting up game instance");
        var settings = new GameSettings
        {
            GenerationMode = GenerationMode.Word,
            Map = ""
        };
        string gameId = IdGenerator.GenerateGameId();
        string port = availablePorts.First().ToString();
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

    private static List<int> GetInitialPorts()
    {
        List<int> ports = new(MAX_GAMES);
        for (int i = START_PORT; i < START_PORT + MAX_GAMES; i++)
        {
            ports.Add(i);
        }

        return ports;
    }

    private async Task RegisterWithMatchmakingServer()
    {
        Oneof_HostServerToMatchmaker request = new()
        {
            Register = new Register
            {
                Port = port
            }
        };

        var content = new ByteArrayContent(request.ToByteArray());
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
        HttpResponseMessage response = await httpClient.PostAsync(
            $"{matchmakingServerAddress}/host/register",
            content);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Unable to connect with matchmaking server. {response}");
        }

        Console.WriteLine("Registered with matchmaking server");
    }
}