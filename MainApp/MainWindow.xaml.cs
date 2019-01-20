using HockeyPlayerDatabase.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
         
   public partial class MainWindow
    {

        private ObservableCollection<Player> _playersList = new ObservableCollection<Player>();
        private readonly HockeyContext _context;
        private Player _selectedPlayer;

        public MainWindow()
        {

            InitializeComponent();

            MenuAbout.Click += MenuAbout_OnClick;
            MenuExportToPdf.Click += MenuExportToPdf_OnClick;
            AddPlayerBtn.Click += AddPlayer_OnClick;
            RemoveBtn.Click += BtnRemove_OnClick;
            EditBtn.Click += BtnEdit_OnClick;
            OpenUrlBtn.Click += BtbOpenUrl_OnClick;

            try
            {
                _context = new HockeyContext();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Problem with connection to the database!");
            }

            foreach (var player in _context.GetPlayers())
            {
                _playersList.Add(player);
            }

            PlayersList.ItemsSource = _playersList;
            RefreshNumberOfFilteredPlayers();
        }

        private void MenuAbout_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Designed by Tomas Urban");
        }

        private void MenuItem_OnExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshNumberOfFilteredPlayers()
        {
            NumberOfFilteredPlayers.Text = "Filtered items:  " + PlayersList.Items.Count + " / " + _context.Players.ToArray().Length;
        }

        private void Filter_OnApplyFilter(object sender, EventArgs e)
        {
            var list = Filter.FilterPlayers(_context.GetPlayers());
            _playersList = new ObservableCollection<Player>();
            foreach (var player in list)
            {
                _playersList.Add(player);
            }
            PlayersList.ItemsSource = _playersList; 
            RefreshNumberOfFilteredPlayers();
        }

        private void AddPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            var newPlayer = new NewPlayer
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newPlayer.Closing += NewPlayer_Closing;
            newPlayer.Show();
        }

        public void NewPlayer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            var context = new HockeyContext();
            _playersList = new ObservableCollection<Player>();
            foreach (var player in context.GetPlayers())
            {
                _playersList.Add(player);
            }
            PlayersList.ItemsSource = _playersList;
            RefreshNumberOfFilteredPlayers();
        }

        private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete selected user?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Players.Remove(_selectedPlayer);
                _context.SaveChanges();
                RefreshList();
            }
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var newPlayer = new NewPlayer(_selectedPlayer)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newPlayer.Closing += NewPlayer_Closing;
            newPlayer.Show();
        }

        private void BtbOpenUrl_OnClick(object sender, RoutedEventArgs e)
        {
            var url = _context.Clubs
                .Where(c => c.Id == _selectedPlayer.ClubId)
                .Select(c => c.Url)
                .First();
            try
            {
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Could not open the site");
            }
        }

        private void PlayersList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedPlayer = (Player)PlayersList.SelectedItem;
            AddPlayerBtn.IsEnabled = true;
            EditBtn.IsEnabled = true;
            RemoveBtn.IsEnabled = true;
            OpenUrlBtn.IsEnabled = true;
        }

        private void MenuExportToPdf_OnClick(object sender, RoutedEventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the XML file  
            // assigned to menu button saveToXML  
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "XML file|*.xml",
                Title = "Save an XML File"
            };

            if (saveFileDialog1.ShowDialog() == true)
            {
                _context.SaveToXml(saveFileDialog1.FileName);
            }
        }
    }
}
