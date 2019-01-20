using System.Collections.Generic;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.Model
{
    public class Club : IClub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }

        public ICollection<Player> Players { get; set; }

        public Club(string name, string address, string url)
        {
            Name = name;
            Address = address;
            Url = url;
        }

        public Club()
        {
        }

        public override string ToString()
        {
            return Name + " (" + Address + ") [" + Url + "]";
        }
    }
}