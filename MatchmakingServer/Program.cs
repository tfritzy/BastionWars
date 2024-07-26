using MatchmakingServer;

Server server = new();
server.StartAcceptingConnections();

while (true)
{
    server.Update();
}