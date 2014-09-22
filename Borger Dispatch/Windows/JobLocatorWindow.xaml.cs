using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for JobLocatorWindow.xaml
    /// </summary>
    public partial class JobLocatorWindow
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
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    FindJob();
                    break;
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
                return (from j in SingerConfigs.CommonDataContext.Jobs where j.Number == number select j).First();
            }
            catch
            {
                return null;
            }
        }
        
    }
}
