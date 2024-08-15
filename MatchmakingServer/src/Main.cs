using MatchmakingServer;

Server server = new(new HttpClient());
await server.SetupAndListen();