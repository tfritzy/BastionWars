namespace KeepLordWarriors;

public static class Constants
{
    public const float KeepHeight = 10f;
    public const float ArrowGravity = 5f;
    public const float ArrowVelocity = 3f;
    public const float ArrowWeakVelocity = 1f;
    public const float ArcherBaseCooldown = 7f;
    public const float ArcherBaseRange = 9f;
    public const float MageBaseRange = 5f;
    public const float ArcherSetupMaxTime = .5f;
    public const float ArcherSetupMinTime = .2f;
    public const int ArrowBaseDamage = 1;

    public static readonly string[] KeepNames = new string[]
    {
        "carlow",
        "cabra",
        "dangan",
        "dromore",
        "moher",
        "tomra",
        "belgard",
        "conn",
        "corr",
        "dalkey",
        "tymon",
        "ellen",
        "kirk",
        "cregg",
        "glinsk",
        "sybil",
        "ross",
        "rynn",
        "adare",
        "black",
        "glin",
        "oola",
        "ardee",
        "roche",
        "darver",
        "turin",
        "tara",
        "trim",
        "leap",
        "doon",
        "roslee",
        "nugent",
        "bargy",
        "ferns",
        "slade",
        "belfast",
        "galgorm",
        "shane",
        "fathom",
        "lurgan",
        "moyry",
        "bangor",
        "bright",
        "clough",
        "dundrum",
        "myra",
        "balfour",
        "caldwell",
        "coole",
        "crom",
        "tully"
    };

    public static class TileCase
    {
        /*
            0000: Full water
            0001: Single corner
            0010: Single corner
            0011: Two adjacent
            0100: Single corner
            0101: Two adjacent
            0110: Two opposite corners
            0111: Three corners
            1000: Single corner
            1001: Two opposite corners
            1010: Two adjacent
            1011: Three corners
            1100: Two adjacent
            1101: Three corners
            1110: Three corners
            1111: Full land
        */

        public static uint FULL_WATER = 0;
        public static uint FULL_LAND = 15;
        public static readonly HashSet<uint> SINGLE_LAND_CORNERS =
            new HashSet<uint>([1, 2, 4, 8]);
        public static readonly HashSet<uint> TWO_LAND_ADJACENT =
            new HashSet<uint>([3, 5, 10, 12]);
        public static readonly HashSet<uint> TWO_LAND_OPPOSITE =
            new HashSet<uint>([6, 9]);
        public static readonly HashSet<uint> THREE_CORNERS =
            new HashSet<uint>([7, 11, 13, 14]);
    }

}