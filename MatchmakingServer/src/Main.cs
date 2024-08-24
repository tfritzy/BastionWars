using HostServer;

Server server = new(new HttpClient());
await server.SetupAndListen();