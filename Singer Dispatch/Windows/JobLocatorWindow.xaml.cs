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
    /// Interaction logic for JobLocatorWindow.xaml
    /// </summary>
    public partial class JobLocatorWindow : Window
    {
        private Job LocatedJob { get; set; }

        public JobLocatorWindow()
        {
            InitializeComponent();

            LocatedJob = null;
        }

        public Job GetJob()
        {
            ShowDialog();

            return LocatedJob;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                FindJob();
            }
        }

        private void FindJob_Click(object sender, RoutedEventArgs e)
        {
            FindJob();
        }

        private void txtJobNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtJobNumber.Background = Brushes.White;
            txtJobNumber.Foreground = Brushes.Black;
        }

        private void FindJob()
        {
            try
            {
                LocatedJob = JobLookup(Convert.ToInt32(txtJobNumber.Text));

                if (LocatedJob == null)
                {
                    throw new Exception("Could not find job");
                }

                Close();
            }
            catch
            {
                txtJobNumber.Background = Brushes.OrangeRed;
                txtJobNumber.Foreground = Brushes.White;
            }

        }

        private Job JobLookup(Int32 number)
        {
            try
            {
                return (from j in SingerConstants.CommonDataContext.Jobs where j.Number == number select j).First();
            }
            catch
            {
                return null;
            }
        }
        
    }
}
