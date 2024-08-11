namespace Helpers
{
    public static class FindRootDir
    {
        public static string Find()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            while (Directory.GetFiles(currentDirectory, "KeepLordWarriors.sln").Length == 0 &&
                   Directory.GetParent(currentDirectory) != null)
            {
                currentDirectory = Directory.GetParent(currentDirectory)!.FullName;
            }

            if (Directory.GetParent(currentDirectory) == null)
            {
                throw new Exception("Could not find the GameServer solution directory.");
            }

            return currentDirectory;
        }
    }
}