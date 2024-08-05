namespace GameServer;

class GameInstance(string id, Schema.GameSettings gameSettings)
{
    public string Id { get; private set; } = id;
    public Schema.GameSettings GameSettings { get; private set; } = gameSettings;

    public void StartGame()
    {
        Console.WriteLine($"Starting game {Id}");
        while (true) ;
    }
}