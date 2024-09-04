using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Net.WebSockets;
using System.Reflection.Metadata;
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
        EnvHelpers.Init();
        string rawAllowlist = EnvHelpers.Get("ALLOWLISTED_HOST_IPS");
        hostIpAllowlist = new HashSet<string>(rawAllowlist.Split(","));
        httpClient = client;
    }

    public async Task SetupAndListen()
    {
        string port = EnvHelpers.Get("PORT");
        string apiUrl = EnvHelpers.Get("API_URL");
        string url = $"{apiUrl}:{port}/";
        HttpListener httpListener = new();
        httpListener.Prefixes.Add(url);
        httpListener.Start();
        Logger.Log("Listening on " + url);

        _routes.Add("/search-for-game", async (HttpListenerContext context) =>
        {
            var body = await ReadBodyPlayer(context);
            Logger.Log("body: " + body);
            if (body == null) return;
            var searchResult = await HandleSearchForGame(body);
            var responseBody = new Oneof_MatchMakerToPlayer
            {
                FoundGame = searchResult.Body,
            };
            context.Response.StatusCode = searchResult.StatusCode;
            WriteBodyForPlayer(responseBody, context.Response);
        });
        _routes.Add("/host/register", async (HttpListenerContext context) =>
        {
            Logger.Log("Received register");
            string ipAddress = context.Request.RemoteEndPoint.Address.ToString();
            var body = await ReadBodyMatchmaker(context);
            if (body == null) return;
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

            string clientUrl = EnvHelpers.Get("CLIENT_URL");
            context.Response.Headers.Add("Access-Control-Allow-Origin", clientUrl);
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
            Logger.Log("Failed to accept connection: " + e.Message);
        }

        await Listen(httpListener);
    }

    public ResponseDetails<Registered> HandleRegisterHost(string ipAddress, Register request)
    {
        if (!hostIpAllowlist.Contains(ipAddress))
        {
            Logger.Log($"Not allowlisted host tried registering: {ipAddress}");
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
            Logger.Log($"Could not parse address: {ipAddress}");
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

        Logger.Log($"Host connected from: {ipAddress} on port {request.Port}");

        return new ResponseDetails<Registered>
        {
            StatusCode = 200,
            Body = new Registered
            {
                Port = request.Port
            }
        };
    }

    public async Task<ResponseDetails<GameFoundForPlayer>> HandleSearchForGame(Oneof_PlayerToMatchmaker request)
    {
        Logger.Log($"Player {request.PlayerId} searched for a game");

        if (string.IsNullOrEmpty(request.PlayerId))
        {
            return new ResponseDetails<GameFoundForPlayer>
            {
                StatusCode = 400,
                Body = null
            };
        }

        if (ConnectedHosts.Count == 0)
        {
            Logger.Log($"No hosts available for {request.PlayerId}!");
            return new ResponseDetails<GameFoundForPlayer>
            {
                StatusCode = 500,
                Body = null
            };
        }


        Oneof_MatchmakerToHostServer placePlayerRequest = new()
        {
            PlacePlayerInGame = new PlacePlayerInGame()
            {
                PlayerId = request.PlayerId,
            }
        };

        string host = ConnectedHosts.First();
        HttpResponseMessage response = await httpClient.PostAsync(
            $"http://{host}/place-player",
            new ByteArrayContent(placePlayerRequest.ToByteArray()));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Logger.Log($"Host {host} returned a {response.StatusCode} removing them.");
            ConnectedHosts.Remove(host);
            return await HandleSearchForGame(request);
        }

        Oneof_HostServerToMatchmaker hstm =
            Oneof_HostServerToMatchmaker.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync());
        Logger.Log("Game found body: " + hstm.GameAvailableOnPort.ToString());

        Logger.Log("TODO: use a host object instead of string splitting");
        string hostIp = host.Split("]:")[0] + "]";
        string address = $"ws://{hostIp}:{hstm.GameAvailableOnPort.Port}";
        Logger.Log($"Telling {request.PlayerId} to join {address}");
        return new ResponseDetails<GameFoundForPlayer>
        {
            Body = new GameFoundForPlayer
            {
                Address = address,
                GameId = hstm.GameAvailableOnPort.GameId,
                PlayerId = hstm.GameAvailableOnPort.PlayerId
            },
            StatusCode = 200,
        };
    }

    private void HandleNotFound(HttpListenerContext context)
    {
        Logger.Log($"Unhandled route: {context.Request.Url}");
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
            Logger.Log("Could not parse body into expected format");
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