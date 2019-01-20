using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for NewPlayer.xaml
    /// </summary>
    public partial class NewPlayer : Window
    {

        private ObservableCollection<Club> _clubsList = new ObservableCollection<Club>();
        private readonly HockeyContext _context;
        private Player _player = new Player();
        private bool _edit = false;

        public NewPlayer()
        {
            InitializeComponent();

            TbKrp.PreviewTextInput += TbKrpOnPreviewTextInput;
            TbBirth.PreviewTextInput += TbBirthOnPreviewTextInput;
            BtnCancel.Click += BtnCancelOnClick;
            BtnOk.Click += BtnOkOnClick;

            try
            {
                _context = new HockeyContext();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Problem with connection to the database!");
            }

            foreach (var club in _context.GetClubs())
            {
                _clubsList.Add(club);
            }
        }

        public NewPlayer(Player player): base()
        {
            InitializeComponent();

            TbKrp.PreviewTextInput += TbKrpOnPreviewTextInput;
            TbBirth.PreviewTextInput += TbBirthOnPreviewTextInput;
            BtnCancel.Click += BtnCancelOnClick;
            BtnOk.Click += BtnOkOnClick;

            _edit = true;
            try
            {
                _context = new HockeyContext();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Problem with connection to the database!");
            }

            foreach (var club in _context.GetClubs())
            {
                _clubsList.Add(club);
            }
            _player = _context.Players.First(p => p.Id == player.Id);
            CbClub.ItemsSource = _clubsList;
            TbKrp.Text = _player.KrpId.ToString();
            TbBirth.Text = _player.YearOfBirth.ToString();
            TbFirstName.Text = _player.FirstName;
            TbLastname.Text = _player.LastName;
            TbTitle.Text = _player.TitleBefore;
            CbClub.SelectedIndex = CbClub.Items.IndexOf(_clubsList.FirstOrDefault(c => c.Id ==_player.ClubId));
            CbCategory.SelectedIndex = (int)_player.AgeCategory;
        }

        private void BtnOkOnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidatePlayer())
            {
                return;
            }

            if (!_edit)
            {
                var krpList = _context.Players.Select(p => p.KrpId).ToList();
                foreach (var i in krpList)
                {
                    if (i == int.Parse(TbKrp.Text))
                    {
                        MessageBox.Show("Krp already exists!!");
                        return;
                    }
                }
                _context.Players.Add(_player);
            }
            _context.SaveChanges();
            Close();
        }

        private void BtnCancelOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TbBirthOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void TbKrpOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void CbAgeCategory(object sender, RoutedEventArgs e)
        {
            CbCategory.ItemsSource = Enum.GetNames(typeof(AgeCategory));
        }


        private void CbClub_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            CbClub.ItemsSource = _clubsList;
        }


        private bool ValidatePlayer()
        {
            if (!ValidateName(TbFirstName.Text) || !ValidateName(TbLastname.Text))
            {
                MessageBox.Show("Players name must me at least three characters long!");
                return false;
            }
            if (CbClub.SelectedItem == null)
            {
                MessageBox.Show("Club must be selected!");
                return false;
            }
            if (CbCategory == null)
            {
                MessageBox.Show("Age category must be selected!");
                return false;
            }

            if (!int.TryParse(TbBirth.Text, out var birth))
            {
                MessageBox.Show("Invalid year of birth!");
                return false;
            }

            if (!int.TryParse(TbKrp.Text, out var krp))
            {
                MessageBox.Show("Invalid KRP!");
                return false;
            }

            _player.FirstName = ToUpperFirstLetter(TbFirstName.Text);
            _player.LastName = ToUpperFirstLetter(TbLastname.Text);
            _player.TitleBefore = TbTitle.Text;
            _player.KrpId = krp;
            _player.AgeCategory = (AgeCategory) Enum.Parse(typeof(AgeCategory), CbCategory.Text);
            _player.YearOfBirth = birth;
            _player.ClubId = (int) CbClub.SelectedValue;
            return true;
        }

        private string ToUpperFirstLetter(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            return name.First().ToString().ToUpper() + name.Substring(1).ToLower();
        }

        private static bool ValidateName(string name)
        {
            return name.Length >= 3;
        }

    }
}
