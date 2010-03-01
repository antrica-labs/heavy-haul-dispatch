using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

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
                
                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                list.Add(cb);
            }
            
            gbVariableDetails.DataContext = null;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (Condition)cb.DataContext;

            TheList.SelectedItem = cb;

            if (SelectedQuote == null || condition == null) return;

            var quoteCondition = new QuoteCondition { Condition = condition, Quote = SelectedQuote };

            SelectedQuote.QuoteConditions.Add(quoteCondition);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (Condition)cb.DataContext;

            TheList.SelectedItem = cb;

            if (SelectedQuote == null || condition == null) return;

            var list = (from c in SelectedQuote.QuoteConditions where c.Condition == condition select c).ToList();

            foreach (var item in list)
            {
                SelectedQuote.QuoteConditions.Remove(item);
            }
        }

        private void TheList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedQuote == null || TheList.SelectedItem == null) return;

            var cb = (CheckBox)TheList.SelectedItem;
            var condition = (Condition)cb.DataContext;

            if (condition == null) return;

            try
            {
                var quoteCondition = (from qc in SelectedQuote.QuoteConditions where qc.Condition == condition select qc).First();

                quoteCondition.Replacement1 = quoteCondition.Replacement1 ?? condition.DefaultVariable1;
                quoteCondition.Replacement2 = quoteCondition.Replacement2 ?? condition.DefaultVariable2;
                quoteCondition.Replacement3 = quoteCondition.Replacement3 ?? condition.DefaultVariable3;

                gbVariableDetails.DataContext = quoteCondition;
            }
            catch
            {
                gbVariableDetails.DataContext = null;
            }
        }
    }
}
