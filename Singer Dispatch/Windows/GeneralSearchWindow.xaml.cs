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
    /// Interaction logic for GeneralSearchWindow.xaml
    /// </summary>
    public partial class GeneralSearchWindow : Window
    {
        private object SelectedEntity;

        public GeneralSearchWindow()
        {
            InitializeComponent();

            SelectedEntity = null;
        }

        public object FindSomething()
        {
            ShowDialog();

            return SelectedEntity;
        }

        private void RunSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
