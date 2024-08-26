using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Net.WebSockets;
using System.Security.Principal;
using DotNetEnv;
using Google.Protobuf;
using Helpers;
using Schema;

namespace HostServer;

public class Server
{
    private readonly HashSet<string> hostIpAllowlist = [];
    private readonly Dictionary<string, Func<HttpListenerContext, Task>> _routes = [];
    public List<string> ConnectedHosts = [];
    private readonly HttpClient httpClient;

    public Server(HttpClient client)
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        string rawAllowlist = Environment.GetEnvironmentVariable("ALLOWLISTED_HOST_IPS")
            ?? throw new Exception("Missing ALLOWLISTED_HOST_IPS in env file");
        hostIpAllowlist = new HashSet<string>(rawAllowlist.Split(","));
        httpClient = client;
    }

    public async Task SetupAndListen()
    {
        string url = Environment.GetEnvironmentVariable("HOSTED_ADDRESS")
            ?? throw new Exception("Missing HOSTED_ADDRESS in env file");

        HttpListener httpListener = new();

        if (String.IsNullOrEmpty(url))
        {
            throw new Exception("HOSTED_ADDRESS environment variable not set.");
        }

        httpListener.Prefixes.Add(url);
        httpListener.Start();
        Console.WriteLine("Listening on " + url);

        _routes.Add("/search-for-game", async (HttpListenerContext context) =>
        {
            string playerId = context.User.Identity.Name;
            var body = await ReadBodyPlayer(context);
            if (body?.SearchForGame == null) return;
            var searchResult = await HandleSearchForGame(playerId, body.SearchForGame);
            var responseBody = new Oneof_MatchMakerToPlayer
            {
                FoundGame = searchResult.Body,
            };
            context.Response.StatusCode = searchResult.StatusCode;
            WriteBodyForPlayer(responseBody, context.Response);
        });
        _routes.Add("/host/register", async (HttpListenerContext context) =>
        {
            string ipAddress = context.Request.RemoteEndPoint.Address.ToString();
            var body = await ReadBodyMatchmaker(context);
            if (body?.Register == null) return;
            var registerResult = HandleRegisterHost(ipAddress, body.Register);
            var responseBody = new Oneof_MatchmakerToHostServer
            {
                Registered = registerResult.Body
            };
            context.Response.StatusCode = registerResult.StatusCode;
            WriteBodyForHostServer(responseBody, context.Response);
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

    public ResponseDetails<Registered> HandleRegisterHost(string ipAddress, Register request)
    {
        if (!hostIpAllowlist.Contains(ipAddress))
        {
            Console.WriteLine($"Not allowlisted host tried registering: {ipAddress}");
            return new ResponseDetails<Registered>
            {
                StatusCode = 400,
                Body = null
            };
        }

        string formattedAddress;
        if (IPAddress.TryParse(ipAddress, out IPAddress? parsedIp))
        {
            if (parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                formattedAddress = $"[{ipAddress}]:{request.Port}";
            }
            else
            {
                formattedAddress = $"{ipAddress}:{request.Port}";
            }
        }
        else
        {
            Console.WriteLine($"Could not parse address: {ipAddress}");
            return new ResponseDetails<Registered>
            {
                StatusCode = 400,
                Body = null
            };
        }

        if (!ConnectedHosts.Contains(formattedAddress))
        {
            ConnectedHosts.Add(formattedAddress);
        }

        Console.WriteLine($"Host connected from: {ipAddress} on port {request.Port}");

        return new ResponseDetails<Registered>
        {
            StatusCode = 200,
            Body = new Registered
            {
                Port = request.Port
            }
        };
    }

    public async Task<ResponseDetails<GameFoundForPlayer>> HandleSearchForGame(string playerId, SearchForGame request)
    {
        Console.WriteLine($"Player {playerId} searched for a game");
        Oneof_MatchmakerToHostServer placePlayerRequest = new()
        {
            PlacePlayerInGame = new PlacePlayerInGame()
            {
                PlayerId = playerId,
            }
        };

        if (ConnectedHosts.Count == 0)
        {
            Console.WriteLine($"No hosts available for {playerId}!");
            return new ResponseDetails<GameFoundForPlayer>
            {
                StatusCode = 500,
                Body = null
            };
        }

        string host = ConnectedHosts.First();
        HttpResponseMessage response = await httpClient.PostAsync(
            $"http://{host}/place-player",
            new ByteArrayContent(placePlayerRequest.ToByteArray()));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Host {host} is actually dead. Removing them.");
            ConnectedHosts.Remove(host);
            return await HandleSearchForGame(playerId, request);
        }

        GameFoundForPlayer gameFound =
            GameFoundForPlayer.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync());

        Console.WriteLine($"Telling {playerId} to join {gameFound.Address}");
        return new ResponseDetails<GameFoundForPlayer>
        {
            Body = gameFound,
            StatusCode = 200,
        };
    }

    private void HandleNotFound(HttpListenerContext context)
    {
        Console.WriteLine($"Unhandled route: {context.Request.Url}");
        context.Response.StatusCode = 404;
        context.Response.Close();
    }

    private static async Task<Oneof_PlayerToMatchmaker?> ReadBodyPlayer(HttpListenerContext context)
    {
        return await ReadBody(context, Oneof_PlayerToMatchmaker.Parser.ParseFrom);
    }

    private static async Task<Oneof_HostServerToMatchmaker?> ReadBodyMatchmaker(HttpListenerContext context)
    {
        return await ReadBody(context, Oneof_HostServerToMatchmaker.Parser.ParseFrom);
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

    private void WriteBodyForHostServer(Oneof_MatchmakerToHostServer message, HttpListenerResponse response)
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

    private void WriteBodyForPlayer(Oneof_MatchMakerToPlayer message, HttpListenerResponse response)
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
}