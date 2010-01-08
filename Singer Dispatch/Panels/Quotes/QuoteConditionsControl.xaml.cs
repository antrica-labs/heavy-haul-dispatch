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

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteConditionsControl.xaml
    /// </summary>
    public partial class QuoteConditionsControl : QuoteUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteConditionsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            TheList.ItemsSource = new ObservableCollection<CheckBox>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            list.Clear();

            if (SelectedQuote == null) return;            
            
            var conditions = from c in Database.Conditions select c;
            var selected = from qc in SelectedQuote.QuoteConditions select qc.Condition;
         
            foreach (var condition in conditions)
            {
                var cb = new CheckBox { Content = condition.Line, DataContext = condition, IsChecked = selected.Contains(condition) };
                
                cb.Checked += new RoutedEventHandler(CheckBox_Checked);
                cb.Unchecked += new RoutedEventHandler(CheckBox_Unchecked);

                list.Add(cb);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (Condition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            var quoteCondition = new QuoteCondition { Condition = condition, Quote = SelectedQuote };

            SelectedQuote.QuoteConditions.Add(quoteCondition);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (Condition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            var list = (from c in SelectedQuote.QuoteConditions where c.Condition == condition select c).ToList();

            foreach (var item in list)
            {
                SelectedQuote.QuoteConditions.Remove(item);
            }
        }
    }
}
