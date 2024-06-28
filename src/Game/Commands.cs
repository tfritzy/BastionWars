namespace KeepLordWarriors;

public static class Commands
{
    public static void HandleCommand(Game game, string command)
    {
        command = command.ToLower();
        var parts = command.Split(' ');
        switch (parts[0])
        {
            case "attack":
                if (!int.TryParse(parts[1], out var x) || !int.TryParse(parts[2], out var y))
                {
                    Console.WriteLine("Invalid attack command. Usage: attack <x> <y>");
                    return;
                }

                game.Map.AttackBastion(x, y);
                break;
            default:
                Console.WriteLine("Unknown command.");
                break;
        }
    }
}