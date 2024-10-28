namespace KeepLordWarriors;

public static class Constants
{
    public const float KeepHeight = 10f;
    public const float ArrowGravity = 3.5f;
    public const float ArrowVelocity = 6f;
    public const float ArrowWeakVelocity = 3f;
    public const float ArcherBaseCooldown = 10f;
    public const float ArcherBaseRange = 6f;
    public const float MageBaseRange = 5f;
    public const float ArcherSetupMaxTime = 1f;
    public const float ArcherSetupMinTime = .2f;
    public const int ArrowBaseDamage = 1;

    public static readonly string[] KeepNames =
    [
        "alford",
        "appleby",
        "arundel",
        "ashby",
        "ayr",
        "ayton",
        "banff",
        "bath",
        "bedale",
        "belvoir",
        "blair",
        "bodiam",
        "bolton",
        "boston",
        "brackley",
        "brecon",
        "bridport",
        "bristol",
        "brough",
        "bungay",
        "calke",
        "calne",
        "campbel",
        "cardiff",
        "carlisle",
        "carlow",
        "chepstow",
        "chester",
        "clare",
        "conwy",
        "corfe",
        "crediton",
        "culzean",
        "dale",
        "dawlish",
        "denbigh",
        "derby",
        "dingwall",
        "diss",
        "dolgellau",
        "dorking",
        "dorset",
        "dover",
        "dudley",
        "dunbar",
        "dundas",
        "durham",
        "elgin",
        "ellon",
        "elstree",
        "elswick",
        "ely",
        "emsworth",
        "essex",
        "fairford",
        "falkirk",
        "filey",
        "forfar",
        "forres",
        "fowey",
        "frodsham",
        "frome",
        "glamis",
        "glossop",
        "godstone",
        "goole",
        "gordon",
        "gosport",
        "grantown",
        "grove",
        "hadley",
        "halstead",
        "harlech",
        "harwich",
        "hayes",
        "henley",
        "hexham",
        "hitchin",
        "holme",
        "holt",
        "honiton",
        "hope",
        "hull",
        "hythe",
        "ilkley",
        "insch",
        "inverness",
        "inwith",
        "irvine",
        "keele",
        "keith",
        "kellie",
        "kelso",
        "kelton",
        "kendal",
        "kendall",
        "kent",
        "keswick",
        "knutsford",
        "lanark",
        "ledbury",
        "leeds",
        "leith",
        "lerwick",
        "lewes",
        "lisburn",
        "louth",
        "ludlow",
        "lyme",
        "lynn",
        "mallow",
        "malton",
        "march",
        "margate",
        "melksham",
        "melrose",
        "melton",
        "mey",
        "montrose",
        "moray",
        "morton",
        "nairn",
        "neath",
        "neston",
        "newark",
        "newbury",
        "newhaven",
        "norwich",
        "oakham",
        "oakley",
        "oban",
        "otley",
        "oundle",
        "oxted",
        "peebles",
        "penarth",
        "penrith",
        "penzance",
        "perth",
        "powis",
        "preston",
        "raby",
        "radnor",
        "ramsey",
        "redhill",
        "ripley",
        "ripon",
        "romsey",
        "ross",
        "rothesay",
        "ruthin",
        "rye",
        "sale",
        "seaford",
        "seaton",
        "selby",
        "selkirk",
        "selsey",
        "settle",
        "skye",
        "slane",
        "stirling",
        "tain",
        "taunton",
        "tenby",
        "tetbury",
        "tewkesbury",
        "thame",
        "thirsk",
        "thorn",
        "thurso",
        "totnes",
        "trim",
        "truro",
        "uppingham",
        "upton",
        "usk",
        "uttoxeter",
        "ventnor",
        "wadebridge",
        "walden",
        "walton",
        "wantage",
        "ware",
        "wareham",
        "warwick",
        "wells",
        "wem",
        "whitby",
        "wick",
        "windsor",
        "witney",
        "woodbridge",
        "yarm",
        "yarmouth",
        "yaxley",
        "yeovil",
        "york",
    ];

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