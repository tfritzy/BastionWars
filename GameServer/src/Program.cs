
using GameServer;
using Schema;

if (args.Length < 2)
{
    throw new ArgumentException("Expected two args. One for gameId and one for game settings");
}

string gameId = args[0];
byte[] settingsBytes = Convert.FromBase64String(args[1]);
GameSettings gameSettings = GameSettings.Parser.ParseFrom(settingsBytes);

GameInstance gameInstance = new(gameId, gameSettings);
gameInstance.StartGame();