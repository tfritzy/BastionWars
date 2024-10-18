namespace KeepLordWarriors.AI;

public static class Constants
{
    public const int ACTION_SIZE = 5;  // 0%, 25%, 50%, 75%, 100%
    public const int NUM_KEEPS = 64;
    public const int MAP_SIZE = 256;
    public const int NUM_TILES = 2; // Traversable or not
    public const int TOTAL_ACTION_SPACE = NUM_KEEPS * NUM_KEEPS * ACTION_SIZE * ACTION_SIZE;
}