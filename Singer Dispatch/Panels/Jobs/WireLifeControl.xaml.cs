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

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for WireLifeControl.xaml
    /// </summary>
    public partial class WireLifeControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public WireLifeControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCompanies.ItemsSource = from c in Database.Companies select c;
        }

        private void NewWireLift_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveWireLift_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
