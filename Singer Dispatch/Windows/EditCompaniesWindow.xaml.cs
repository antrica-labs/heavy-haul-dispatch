using System.Linq;
using System.Windows;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for EditCompanies.xaml
    /// </summary>
    public partial class EditCompaniesWindow
    {
        public SingerDispatchDataContext Database { get; set; }

        public EditCompaniesWindow()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompaniesGrid.ItemsSource = from c in Database.Companies orderby c.Name select c;
            CBSearch.ItemsSource = CompaniesGrid.ItemsSource;

            CBSearch.Focus();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CompaniesGrid_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CBSearch_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CompaniesGrid.ScrollIntoView(CBSearch.SelectedItem);
            CompaniesGrid.SelectedItem = CBSearch.SelectedItem;
        }
    }
}
