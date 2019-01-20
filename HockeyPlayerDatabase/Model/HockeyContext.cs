using System.Collections.Concurrent;
using System.Xml.Linq;
using HockeyPlayerDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HockeyPlayerDatabase.Model
{
    public class HockeyContext : DbContext , IHockeyReport<Club,Player>
    {
        public HockeyContext()
            : base("name=HockeyContext")
        {
        }

        public HockeyContext(string ConnnectionString)
            : base(ConnnectionString)
        {
        }
        
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }

        public IEnumerable<int> GetBiggestClubPlayerAges()
        {
            int? biggestClub = Clubs.OrderByDescending(club => club.Players.Count)
                    .Select(club => club.Id)
                    .First();
            return Players
                .Where(player => player.ClubId == biggestClub)
                .Select(player => DateTime.Now.Year - player.YearOfBirth)
                .ToList();
        }

        public IQueryable<Club> GetClubs()
        {
            return Clubs;
        }

        public IEnumerable<IGrouping<int, Player>> GetGroupedPlayersByYearOfBirth()
        {
            IEnumerable<IGrouping<int, Player>> result = new List<IGrouping<int, Player>>();
            return Players
                .GroupBy(player => player.YearOfBirth);
        }
        public Player GetYoungestPlayer()
        {
            return Players
                .OrderByDescending(pl => pl.YearOfBirth)
                .ThenByDescending(pl => pl.KrpId)
                .First();
        }

        public Player GetOldestPlayer()
        {
            return Players
                .OrderBy(pl => pl.YearOfBirth)
                .ThenBy(pl => pl.KrpId)
                .First();
        }

        public double GetPlayerAverageAge()
        {
            return Math.Round(Players
                .Average(pl => DateTime.Now.Year - pl.YearOfBirth),2);
        }


        public IQueryable<Player> GetPlayers()
        {
            return Players;
        }

        public IEnumerable<Player> GetPlayersByAge(int minAge, int maxAge)
        {
            return Players
                .Where(pl => (DateTime.Now.Year - pl.YearOfBirth) >= minAge &&
                             (DateTime.Now.Year - pl.YearOfBirth) <= maxAge);
        }

        public IDictionary<AgeCategory, ReportResult> GetReportByAgeCategory()
        {
            IDictionary<AgeCategory, ReportResult> report = new Dictionary<AgeCategory, ReportResult>();
            var playersByGroup = Players
                .GroupBy(player => player.AgeCategory).OrderBy(grp => grp.Count()).Select(grp => grp.Key);
            foreach (var group in playersByGroup)
            {
                if (group == null) continue;
                report.Add((AgeCategory)group,GetReport(Players.Where(p => p.AgeCategory == (AgeCategory)group)));
            }
            return report;
        }

        private IEnumerable<Player> GetPlayersByClub(int clubId)
        {
            return Players
                .Where(player => player.ClubId == clubId)
                .ToList();
        }

        public IEnumerable<Player> GetPlayersByAgeCategory(AgeCategory category)
        {
            return Players
                .Where(player => player.AgeCategory == category)
                .ToList();
        }

        private ReportResult GetReport(IEnumerable<Player> players)
        {
            var enPl = players.ToList();
            var totalPlayerCount = enPl.Count;
            double? averagePlayerAge = Math.Round(enPl
                .DefaultIfEmpty().Average(pl => (int?)(DateTime.Now.Year - pl.YearOfBirth) ?? 0.0), 2);
            var pom = enPl
                .OrderByDescending(player => player.YearOfBirth)
                .ThenByDescending(player => player.KrpId)
                .Select(player => player)
                .First();
            var youngestPlayer = pom.FirstName + ' ' + pom.LastName;
            var pom1 = enPl
                .OrderBy(player => player.YearOfBirth)
                .ThenBy(player => player.KrpId)
                .Select(player => player)
                .First();
            var oldestPlayer = pom1.FirstName + ' ' + pom1.LastName;
            var youngestPlayerAge = enPl
                .Min(player => DateTime.Now.Year - player.YearOfBirth);
            var oldestPlayerAge = enPl
                .Max(player => DateTime.Now.Year - player.YearOfBirth);

            return new ReportResult(totalPlayerCount, averagePlayerAge, youngestPlayer, oldestPlayer, youngestPlayerAge, oldestPlayerAge);
        }

        public ReportResult GetReportByClub(int clubId)
        {
            var clubPlayers = Players.Where(p => p.ClubId == clubId);
            return !clubPlayers.Any() ? new ReportResult(0, 0, null, null, 0, 0) : GetReport(clubPlayers);
        }

        public IEnumerable<Club> GetSortedClubs(int maxResultCount)
        {
            return Clubs?.OrderByDescending(club => club.Players.Count)
                .Take(maxResultCount);
        }

        public IEnumerable<Player> GetSortedPlayers(int maxResultCount)
        {
            return Players?.OrderBy(pl => pl.LastName)
                .ThenByDescending(pl => pl.FirstName)
                .Take(maxResultCount);
        }


        public void SaveToXml(string fileName)
        {
            var xmlClubs = new XElement("Clubs");
            foreach (var club in Clubs)
            {
                xmlClubs.Add(new XElement("Club",
                new XElement("Name", club.Name),
                new XElement("Address", club.Address),
                new XElement("Url", club.Url)));
            }

            var xmlPLayers = new XElement("Players");
            foreach (var player in Players)
            {
                xmlPLayers.Add(new XElement("Players",
                    new XElement("FirstName", player.FirstName),
                    new XElement("LastName", player.LastName),
                    new XElement("TitleBefore", player.TitleBefore),
                    new XElement("YearOfBirth", player.YearOfBirth),
                    new XElement("AgeCategory", player.AgeCategory),
                    new XElement("KrpId", player.KrpId),
                    new XElement("Club", player.ClubId)));
            }
            
            var xmlTree = new XElement("Root",
                new XElement(xmlClubs),
                new XElement(xmlPLayers));
            
            xmlTree.Save(fileName);
        }
    }
}