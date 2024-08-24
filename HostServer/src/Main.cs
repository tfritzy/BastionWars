using HostServer;

Server host = new(new HttpClient());
await host.Setup();
await host.Listen();