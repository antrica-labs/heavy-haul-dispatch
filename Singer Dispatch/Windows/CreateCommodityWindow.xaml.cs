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

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CreateCommodityWindow.xaml
    /// </summary>
    public partial class CreateCommodityWindow : Window
    {
        private Commodity _commodity;
        private SingerDispatchDataContext _database;

        public CreateCommodityWindow(SingerDispatchDataContext database)
        {
            InitializeComponent();

            _database = database;
            _commodity = new Commodity();

            DataContext = _commodity;
        }

        public Commodity CreateCommodity()
        {
            ShowDialog();

            return _commodity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CreateCompany_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
