using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteInclusionsControl.xaml
    /// </summary>
    public partial class QuoteInclusionsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteInclusionsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            TheList.ItemsSource = new ObservableCollection<CheckBox>();
        }

        private void QuoteUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            list.Clear();

            if (SelectedQuote == null) return;

            var inclusions = from c in Database.Inclusions select c;
            var selected = from qc in SelectedQuote.QuoteInclusions select qc.Inclusion;

            foreach (var inclusion in inclusions)
            {
                var cb = new CheckBox { Content = inclusion.Line, DataContext = inclusion, IsChecked = selected.Contains(inclusion) };

                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                list.Add(cb);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var inclusion = (Inclusion)cb.DataContext;

            if (SelectedQuote == null || inclusion == null) return;

            var quoteInclusion = new QuoteInclusion { Inclusion = inclusion, Quote = SelectedQuote };

            SelectedQuote.QuoteInclusions.Add(quoteInclusion);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var inclusion = (Inclusion)cb.DataContext;

            if (SelectedQuote == null || inclusion == null) return;

            var list = (from c in SelectedQuote.QuoteInclusions where c.Inclusion == inclusion select c).ToList();

            foreach (var item in list)
            {
                SelectedQuote.QuoteInclusions.Remove(item);
            }
        }
    }
}
