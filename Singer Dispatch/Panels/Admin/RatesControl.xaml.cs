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
using Microsoft.Windows.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for RatesControl.xaml
    /// </summary>
    public partial class RatesControl : UserControl
    {
        public SingerDispatchDataContext Database { get; set; }
      
        public RatesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            dgRates.ItemsSource = new ObservableCollection<Rate>(from r in Database.Rates select r);
        }

        private void NewRate_Click(object sender, RoutedEventArgs e)
        {
            var rate = new Rate();

            Database.Rates.InsertOnSubmit(rate);
            ((ObservableCollection<Rate>)dgRates.ItemsSource).Insert(0, rate);
            dgRates.SelectedItem = rate;
        }

        private void RemoveRate_Click(object sender, RoutedEventArgs e)
        {
            var rate = (Rate)dgRates.SelectedItem;

            if (rate == null)
                return;

            Database.Rates.DeleteOnSubmit(rate);
            ((ObservableCollection<Rate>)dgRates.ItemsSource).Remove(rate);

            Database.SubmitChanges();
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            Database.SubmitChanges();            
        }        
    }
}
