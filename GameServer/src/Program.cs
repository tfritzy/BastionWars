
using GameServer;
using Helpers;
using Schema;

string gameId;
GameSettings gameSettings;
string port;
if (args.Length == 3)
{
    gameId = args[0];
    byte[] settingsBytes = Convert.FromBase64String(args[1]);
    gameSettings = GameSettings.Parser.ParseFrom(settingsBytes);
    port = args[2];
}
else
{
    gameId = IdGenerator.GenerateGameId();
    gameSettings = new GameSettings
    {
        GenerationMode = GenerationMode.Word,
        Map = Maps.Map,
    };
    port = "7250";
}

GameInstance gameInstance = new(gameId, gameSettings);
gameInstance.StartGame();