using System.Net;
using DotNetEnv;
using GameServer;
using Google.Protobuf;
using Helpers;
using HostServer;
using Schema;

namespace MatchmakingServer;

public class Server
{
    private readonly Dictionary<string, Func<HttpListenerContext, Task>> _routes = [];
    private readonly HttpClient httpClient;
    public readonly List<GameInstanceDetails> Games = [];
    private readonly List<int> availablePorts;
    private readonly string matchmakingServerAddress;

    private const int START_PORT = 1750;
    private const int MAX_GAMES = 100;

    public Server(HttpClient client)
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        matchmakingServerAddress = Environment.GetEnvironmentVariable("MATCHMAKING_SERVER_ADDRESS")
            ?? throw new Exception("MATCHMAKING_SERVER_ADDRESS environment variable not set.");

        httpClient = client;
        availablePorts = GetInitialPorts();

        StartGameInstance();
    }

    public async Task SetupAndListen()
    {
        string url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("Missing HOSTED_ADDRESS in env file");

        HttpListener httpListener = new();

        if (string.IsNullOrEmpty(url))
        {
            throw new Exception("HOSTED_ADDRESS environment variable not set.");
        }

        httpListener.Prefixes.Add(url);
        httpListener.Start();
        Console.WriteLine("Listening on " + url);

        _routes.Add("/place-player", async (HttpListenerContext context) =>
        {
            string playerId = context.User.Identity.Name;
            var body = await ReadBody<PlacePlayerInGame>(context);
            if (body == null) return;
            var gameDetails = await HandlePlacePlayer(body);
            var responseBody = new Oneof_HostServerToMatchmaker
            {
                GameAvailableOnPort = gameDetails.Body
            };
            context.Response.StatusCode = gameDetails.StatusCode;
            WriteBodyForMatchmaker(responseBody, context.Response);
        });

        await Listen(httpListener);
    }

    public async Task Listen(HttpListener httpListener)
    {
        try
        {
            var context = await httpListener.GetContextAsync();
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
        catch (Exception e)
        {
            Console.WriteLine("Failed to accept connection: " + e.Message);
        }

        await Listen(httpListener);
    }


    public async Task<ResponseDetails<GameAvailableOnPort>> HandlePlacePlayer(PlacePlayerInGame placePlayer)
    {
        GameInstanceDetails details = GetGameForPlayer(placePlayer);

        return new ResponseDetails<GameAvailableOnPort>
        {
            Body = new GameAvailableOnPort()
            {
                Port = details.Port,
                GameId = details.Id,
                PlayerId = placePlayer.PlayerId,
            },
            StatusCode = 200,
        };
    }

    private void HandleNotFound(HttpListenerContext context)
    {
        context.Response.StatusCode = 404;
        context.Response.Close();
    }

    private async Task<T?> ReadBody<T>(HttpListenerContext context) where T : IMessage<T>, new()
    {
        try
        {
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string requestBody = await reader.ReadToEndAsync();
                return JsonParser.Default.Parse<T>(requestBody);
            }
        }
        catch
        {
            context.Response.StatusCode = 400;
            context.Response.Close();
            return default(T);
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

}