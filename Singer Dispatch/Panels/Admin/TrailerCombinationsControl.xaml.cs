using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            var list = (ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource;
            var combination = new TrailerCombination();

            try
            {
                combination.Rate = (from r in Database.Rates select r).First();
            }
            catch
            {}

            Database.TrailerCombinations.InsertOnSubmit(combination);
            list.Insert(0, combination);
            dgCombinations.SelectedItem = combination;
            dgCombinations.ScrollIntoView(combination);

            DataGridHelper.GetCell(dgCombinations, dgCombinations.SelectedIndex, 0).Focus();
        }

        private void RemoveCombination_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource;
            var combination = (TrailerCombination)dgCombinations.SelectedItem;

            if (combination == null) return;

            Database.TrailerCombinations.DeleteOnSubmit(combination);
            list.Remove(combination);

            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == Microsoft.Windows.Controls.DataGridEditAction.Commit)
            {
                try
                {
                    Database.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                    SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }        
    }

    public class RatesDropList : ObservableCollection<Rate>
    {
    }
}
