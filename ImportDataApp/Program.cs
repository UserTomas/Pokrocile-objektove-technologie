using System;
using CsvHelper;

namespace ImportDataApp
{
    internal class Program
    {
        private static bool _deleteData = false;
        private static string _pathToClubs;
        private static string _pathToPlayers;

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                return 1;
            }
            
            for (var i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
                switch (args[i])
                {
                    case "-clubs":
                        _pathToClubs = args[i + 1];
                        break;
                    case "-players":
                        _pathToPlayers = args[i + 1];
                        break;
                    case "-clearDatabase":
                        _deleteData = true;
                        break;
                    default:
                        continue;
                }
            }
            var import = new ImportDb();

            return !import.CleanDatabase(_deleteData) ? 1 :
                !import.ImportClubs(_pathToClubs) ? 1 :
                !import.ImportPlayers(_pathToPlayers) ? 1 : 0;

        }
    }
}
