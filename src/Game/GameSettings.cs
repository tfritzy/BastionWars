namespace KeepLordWarriors;

public enum GenerationMode
{
    AutoAccrue,
    Word
};

public struct GameSettings
{
    public GenerationMode GenerationMode { get; private set; }
    public string Map { get; private set; }

    public GameSettings(GenerationMode mode, string map)
    {
        GenerationMode = mode;
        Map = map;
    }
}