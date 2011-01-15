using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            Database = SingerConfigs.CommonDataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompaniesGrid.ItemsSource = from c in Database.Companies orderby c.Name select c;
            CmbSearch.ItemsSource = CompaniesGrid.ItemsSource;

            CmbSearch.Focus();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CompaniesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CmbSearch_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CmbSearch.SelectedItem == null) return;

            CompaniesGrid.ScrollIntoView(CmbSearch.SelectedItem);
            CompaniesGrid.SelectedItem = CmbSearch.SelectedItem;
        }
    }
}
