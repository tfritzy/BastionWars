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

    public Server()
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);

        string rawAllowlist = Environment.GetEnvironmentVariable("ALLOWLISTED_HOST_IPS")
            ?? throw new Exception("Missing ALLOWLISTED_HOST_IPS in env file");
        hostIpAllowlist = new HashSet<string>(rawAllowlist.Split(","));
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

        _routes.Add("/searchForGame", HandleSearchForGame);
        _routes.Add("/host/register", async (HttpListenerContext context) =>
        {
            string ipAddress = context.Request.RemoteEndPoint.Address.ToString();
            var body = await ReadBody<Register>(context);
            if (body == null) return;
            var response = HandleRegisterHost(ipAddress, body);
            context.Response.StatusCode = response.StatusCode;
            // Write body.
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

        ConnectedHosts.Add($"{ipAddress}:{request.Port}");
        return new ResponseDetails<Registered>
        {
            StatusCode = 200,
            Body = new Registered
            {
                Port = request.Port
            }
        };
    }

    public async Task HandleSearchForGame(HttpListenerContext context)
    {
        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = 405;
            context.Response.Close();
        }

        using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        {
            string requestBody = await reader.ReadToEndAsync();
            var searchForGame = JsonParser.Default.Parse<SearchForGame>(requestBody);

            // Process the SearchForGame request here
            // ...

            var placePlayerRequest = new Oneof_MatchmakerToHostServer
            {
                PlacePlayerInGame = new PlacePlayerInGame()
                {
                    PlayerId = context.User?.Identity?.Name,
                }
            };

            context.Response.StatusCode = 200;
            context.Response.Close();
        }
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
}