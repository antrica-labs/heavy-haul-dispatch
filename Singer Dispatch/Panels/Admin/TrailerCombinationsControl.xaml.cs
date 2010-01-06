using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for TrailerCombinationsControl.xaml
    /// </summary>
    public partial class TrailerCombinationsControl : UserControl
    {
        public SingerDispatchDataContext Database { get; set; }
        public IEnumerable<Rate> Rates { get; set; }

        public TrailerCombinationsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            var provider = (ObjectDataProvider)FindResource("RatesDropList");

            if (provider != null)
            {
                var rates = from r in Database.Rates select r;
                var list = (RatesDropList)provider.Data;

                list.Clear();

                foreach (var rate in rates)
                {
                    list.Add(rate);
                }            
            }

            dgCombinations.ItemsSource = new ObservableCollection<TrailerCombination>(from tc in Database.TrailerCombinations select tc);
        }

        private void NewCombination_Click(object sender, RoutedEventArgs e)
        {
            var tc = new TrailerCombination();

            Database.TrailerCombinations.InsertOnSubmit(tc);
            ((ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource).Insert(0, tc);
            dgCombinations.SelectedItem = tc;

            DataGridHelper.GetCell(dgCombinations, dgCombinations.SelectedIndex, 0).Focus();
        }

        private void RemoveCombination_Click(object sender, RoutedEventArgs e)
        {
            var tc = (TrailerCombination)dgCombinations.SelectedItem;

            if (tc == null)
                return;

            Database.TrailerCombinations.DeleteOnSubmit(tc);
            ((ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource).Remove(tc);

            Database.SubmitChanges();
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            Database.SubmitChanges();
        }        
    }

    public class RatesDropList : ObservableCollection<Rate>
    {
    }
}
