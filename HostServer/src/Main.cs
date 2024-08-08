using Helpers;
using HostServer;

LiveWebSocketFactory factory = new();
Host host = new(factory);
await host.Setup();
await host.ListenLoop();