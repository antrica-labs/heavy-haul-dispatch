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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TheList.ItemsSource = from c in Database.Conditions select c;
        }
    }
}
