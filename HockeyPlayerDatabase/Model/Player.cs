using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.Model
{
    public class Player : IPlayer
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + ' ' + LastName;
        public string TitleBefore { get; set; }
        public int YearOfBirth { get; set; }
        public int KrpId { get; set; }
        public AgeCategory? AgeCategory { get; set; }
        public int? ClubId { get; set; }

        public virtual Club MyClub { get; set; }

        public Player()
        {
        }

        public Player(string firstName, string lastName, string titleBefore, int yearOfBirth, int krpId, AgeCategory? ageCategory, int? clubId)
        {
            FirstName = firstName;
            LastName = lastName;
            TitleBefore = titleBefore;
            YearOfBirth = yearOfBirth;
            KrpId = krpId;
            AgeCategory = ageCategory;
            ClubId = clubId;
        }
        
        public override string ToString()
        {
            var clubName = MyClub == null ? "" : MyClub.Name;
            return KrpId + ".\t" + TitleBefore + " " + FullName + " ("  + YearOfBirth + ", " +
                   AgeCategory + "), club: " + clubName;
        }
    }
}