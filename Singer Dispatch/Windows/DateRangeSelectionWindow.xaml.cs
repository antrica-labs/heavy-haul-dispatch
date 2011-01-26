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
    /// Interaction logic for DateRangeSelectionWindow.xaml
    /// </summary>
    public partial class DateRangeSelectionWindow : Window
    {
        private DateRange Range;
        private bool Submitted;

        public DateRangeSelectionWindow()
        {
            InitializeComponent();

            Range = new DateRange();
            Submitted = false;

            DataContext = Range;
        }

        public DateRange GetDates()
        {
            Range.EndDate = DateTime.Today;
            Range.StartDate = Range.EndDate.AddDays(-30);

            ShowDialog();

            return Range;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Submitted = false;
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Submitted)
                Range = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Submitted = true;

            Close();
        }
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
