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

namespace MatchmakingServer;

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
            var body = await ReadBody<SearchForGame>(context);
            if (body == null) return;
            var searchResult = await HandleSearchForGame(playerId, body);
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
            var body = await ReadBody<Register>(context);
            if (body == null) return;
            var registerResult = HandleRegisterHost(ipAddress, body);
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
        Oneof_MatchmakerToHostServer placePlayerRequest = new()
        {
            PlacePlayerInGame = new PlacePlayerInGame()
            {
                PlayerId = playerId,
            }
        };

        string host = ConnectedHosts.First();
        Console.WriteLine($"http://{host}/place-player");
        HttpResponseMessage response = await httpClient.PostAsync(
            $"http://{host}/place-player",
            new ByteArrayContent(placePlayerRequest.ToByteArray()));

        GameFoundForPlayer gameFound =
            GameFoundForPlayer.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync());

        return new ResponseDetails<GameFoundForPlayer>
        {
            Body = gameFound,
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