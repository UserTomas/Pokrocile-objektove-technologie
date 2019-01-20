using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace ImportDataApp
{
    class ImportDb
    {
        //path format "C:\Users\tomas\Documents\Zoznam-klubov.csv"
        public bool ImportClubs(string pathToClubs)
        {
            if (string.IsNullOrEmpty(pathToClubs)) return true;
            var clubList = new List<Club>();
            var sr = new StreamReader(pathToClubs);
            var line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                var tokens = line.Split(';');
                clubList.Add(new Club(tokens[0], tokens[1], tokens[2]));
            }
            sr.Close();

            foreach (var club in clubList)
            {
                Console.WriteLine(club.ToString());
            }
            using (var context = new HockeyContext())
            {
                context.Clubs.AddRange(clubList);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        private static AgeCategory? GetCategory(string category)
        {
            switch (category)
            {
                case "Kadeti":
                    return AgeCategory.Cadet;
                case "Dorastenci":
                    return AgeCategory.Midgest;
                case "Juniori":
                    return AgeCategory.Junior;
                case "Seniori":
                    return AgeCategory.Senior;
                default:
                    return null;
            }
        }

        private static string ToUpperFirstLetter(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            return name.First().ToString().ToUpper() + name.Substring(1).ToLower();
        }

        public bool ImportPlayers(string pathToPlayers)
        {
            if (string.IsNullOrEmpty(pathToPlayers)) return true;
            using (var context = new HockeyContext())
            {
                var sr = new StreamReader(pathToPlayers);
                var playerList = new List<Player>();
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    var tokens = line.Split(';');
                    int krpId = int.Parse(tokens[4]);
                    if (context.Players.Any(x => x.KrpId == krpId)) continue;
                    var player = new Player(tokens[1], ToUpperFirstLetter(tokens[0]), tokens[2], int.Parse(tokens[3]),
                                            krpId, GetCategory(tokens[6]), GetClubId(tokens[5]));
                    playerList.Add(player);
                }
                sr.Close();

                foreach (var player in playerList)
                {
                    Console.WriteLine(player.ToString());
                }
                context.Players.AddRange(playerList);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        private static int? GetClubId(string clubName)
        {
            using (var context = new HockeyContext())
            {
                if (!context.Clubs.Any(c => c.Name == clubName)) return null;
                {
                    var clubs = context.Clubs.First(b => b.Name == clubName);
                    return clubs.Id;
                }
            }
        }


        public bool CleanDatabase(bool deleteData)
        {
            if (!deleteData) return true;
                        
            try
            {
                using (var context = new HockeyContext())
                {
                    context.Players.RemoveRange(context.Players);
                    context.Clubs.RemoveRange(context.Clubs);
                    
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

    }
}
