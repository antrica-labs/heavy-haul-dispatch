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
using System.Windows.Shapes;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for EditCompanies.xaml
    /// </summary>
    public partial class EditCompaniesWindow : Window
    {
        public SingerDispatchDataContext Database { get; set; }

        public EditCompaniesWindow()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompaniesGrid.ItemsSource = from c in Database.Companies select c;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CompaniesGrid_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            Database.SubmitChanges();
        }
    }
}
