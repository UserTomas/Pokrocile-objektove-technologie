using System;
using System.Collections.Generic;
using System.Linq;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace OutputLinqTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new HockeyContext())
            {
                ConsoleOutput(context);
            }

            Console.ReadKey();
        }

        public static void ConsoleOutput(HockeyContext context)
        {
            Console.WriteLine($"GetClubs: {context.GetClubs().Count()}");
            foreach (var club in context.GetClubs())
            {
                Console.WriteLine(club.ToString());
            }

            Console.WriteLine($"GetPlayers: {context.GetPlayers().Count()}");
            foreach (var cb in context.GetPlayers())
            {
                Console.WriteLine(cb.ToString());
            }

            
            Console.WriteLine("GetSortedClubs(5)");
            var result = context.GetSortedClubs(5);
            foreach (var club1 in result)
            {
                Console.WriteLine(club1.ToString());
            }

            Console.WriteLine("GetSortedPlayers(10)");
            var result1 = context.GetSortedPlayers(10);
            foreach (var club1 in result1)
            {
                Console.WriteLine(club1.ToString());
            }

            Console.WriteLine($"GetYoungestPlayer");
            Console.WriteLine(context.GetYoungestPlayer().ToString());

            Console.WriteLine($"GetOldestPlayer");
            Console.WriteLine(context.GetOldestPlayer().ToString());

            Console.WriteLine($"GetBiggestClubByAge");
            var query = context.GetPlayersByAge(20, 23);
            foreach (var pl in query)
            {
                Console.WriteLine(pl);
            }


            Console.WriteLine($"GetGroupedPlayersByYearOfBirth");
            var res1 = context.GetGroupedPlayersByYearOfBirth();
            foreach (var re in res1)
            {
                Console.WriteLine($"{re.Key} ({re.Count()}):");
                foreach (var player in re)
                {
                    Console.WriteLine(player.ToString());
                }
            }

            Console.WriteLine($"GetPlayersAverageAge");
            Console.WriteLine($"{context.GetPlayerAverageAge()}");

            Console.WriteLine($"GetAveragePlayerAge(33,35)");
            var avgPlAge = context.GetPlayersByAge(33, 35);
            foreach (var player in avgPlAge)
            {
                Console.WriteLine(player.ToString());
            }

            Console.WriteLine($"GetReportByClub(1)");
            Console.WriteLine(context.GetReportByClub(280));

            Console.WriteLine($"GetReportByClub(2)");
            Console.WriteLine(context.GetReportByClub(281));

            Console.WriteLine($"GetReportByAgeCategory");
            var cat = context.GetReportByAgeCategory();
            foreach (var player in cat)
            {
                Console.WriteLine($"{player.Key} ({cat.Count}):");
                Console.WriteLine(player.Value);
            }

        }
    }
}
