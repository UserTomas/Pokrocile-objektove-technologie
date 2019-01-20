using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter
    {

        public event EventHandler ApplyFilter;
        
        public Filter()
        {
            InitializeComponent();
            TbYearFrom.PreviewTextInput += TbYearFromOnPreviewTextInput;
            TbYearTo.PreviewTextInput += TbYearToOnPreviewTextInput;
            TbKrp.PreviewTextInput += TbKrpOnPreviewTextInput;
        }

        private void TbKrpOnPreviewTextInput(object sender, TextCompositionEventArgs e)

        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void TbYearToOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void TbYearFromOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

      
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilter?.Invoke(this, e);
        }

        private List<AgeCategory> AgeFilter()
        {
            var ageFilter = new List<AgeCategory>();
            if (CbCadet.IsChecked != null && (bool) CbCadet.IsChecked)
                ageFilter.Add(AgeCategory.Cadet);
            if (CbJunior.IsChecked != null && (bool)CbJunior.IsChecked)
                ageFilter.Add(AgeCategory.Junior);
            if (CbMidges.IsChecked != null && (bool)CbMidges.IsChecked)
                ageFilter.Add(AgeCategory.Midgest);
            if (CbSenior.IsChecked != null && (bool)CbSenior.IsChecked)
                ageFilter.Add(AgeCategory.Senior);
            if (CbCadet.IsChecked != null && (bool)!CbCadet.IsChecked && CbJunior.IsChecked != null && (bool)!CbJunior.IsChecked &&
                CbMidges.IsChecked != null && (bool)!CbMidges.IsChecked && CbSenior.IsChecked != null && (bool)!CbSenior.IsChecked)
            {
                ageFilter.Add(AgeCategory.Cadet);
                ageFilter.Add(AgeCategory.Junior);
                ageFilter.Add(AgeCategory.Senior);
                ageFilter.Add(AgeCategory.Midgest);
                return ageFilter;
            }
            
            return ageFilter;
        }

        public IEnumerable<Player> FilterPlayers(IEnumerable<Player> list)
        {
            var ageCategory = AgeFilter();
            if(!int.TryParse(TbYearFrom.Text, out var yearFrom)) yearFrom = 1900;
            if(!int.TryParse(TbYearTo.Text, out var yearTo)) yearTo = 20000;
            var lists = list
                .Where(p => p.FirstName.ToLower().Contains(TbFirstName.Text.ToLower()))
                .Where(p => p.LastName.ToLower().Contains(TbLastname.Text.ToLower()))
                .Where(p => p.KrpId.ToString().Contains(TbKrp.Text))
                .Where(p => p.YearOfBirth >= yearFrom && p.YearOfBirth <= yearTo)
                .Where(p => p.AgeCategory != null && ageCategory.Contains((AgeCategory) p.AgeCategory));

            if (TbClub.Text != "")
                lists = lists
                    .Where(p => p.MyClub != null)
                    .Where(p => p.MyClub.Name.ToLower().Contains(TbClub.Text.ToLower()));

            return lists;
        }
    }
}
