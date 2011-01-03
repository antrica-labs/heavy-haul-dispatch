using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteConditionsControl.xaml
    /// </summary>
    public partial class QuoteConditionsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteConditionsControl()
        {
            InitializeComponent();
            
            TheList.ItemsSource = new ObservableCollection<CheckBox>();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            list.Clear();

            if (SelectedQuote == null) return;

            var conditions = from c in Database.Conditions where c.Archived != true select c;

            foreach (var condition in conditions)
            {
                QuoteCondition qc;
                bool isChecked;

                var copy = condition;
                var query = from s in SelectedQuote.QuoteConditions where s.ConditionID == copy.ID select s;

                if (query.Count() > 0)
                {
                    qc = query.First();
                    isChecked = true;
                }
                else
                {
                    qc = new QuoteCondition { Line = condition.Line, ConditionID = condition.ID };
                    isChecked = false;
                }                

                var cb = new CheckBox { DataContext = qc, IsChecked = isChecked };
                var tb = new TextBox { Style = (Style)TryFindResource("LongText") };
                var binding = new Binding("Line");

                tb.GotMouseCapture += TextBox_GotFocus;
                tb.GotFocus += TextBox_GotFocus;

                tb.SetBinding(TextBox.TextProperty, binding);

                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                cb.Content = tb;
                
                list.Add(cb);
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtbox = sender as TextBox;

            if (txtbox == null) return;

            var checkbox = txtbox.Parent as CheckBox;

            if (checkbox == null) return;

            checkbox.IsChecked = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (QuoteCondition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            if (!SelectedQuote.QuoteConditions.Contains(condition))
                SelectedQuote.QuoteConditions.Add(condition);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (QuoteCondition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            if (SelectedQuote.QuoteConditions.Contains(condition))
                SelectedQuote.QuoteConditions.Remove(condition);
        }
    }
}
