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
using SingerDispatch.Database;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for NewJobNumberWindow.xaml
    /// </summary>
    public partial class NewJobNumberWindow : Window
    {
        private int? NewJobNumber { get; set; }

        public NewJobNumberWindow()
        {
            InitializeComponent();

            NewJobNumber = null;
        }

        public int? CreateJobNumber()
        {
            ShowDialog();

            return NewJobNumber;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    PickJobNumber();
                    break;
            }
        }

        private void AutoGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var number = EntityHelper.GenerateJobNumber(SingerConfigs.CommonDataContext);

                txtNewJobNumber.Text = number.ToString();
                NewJobNumber = number;

                Close();
            }
            catch
            {
                InvalidateNumber();
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            PickJobNumber();
        }

        private void txtNewJobNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            BeginAcceptingNumber();
        }

        private void PickJobNumber()
        {
            var success = AttemptManualEntry();

            if (success)
                Close();
            else
                InvalidateNumber();
        }

        private bool AttemptManualEntry()
        {
            try
            {
                var number = Convert.ToInt32(txtNewJobNumber.Text);

                EntityHelper.SuggestJobNumber(number, SingerConfigs.CommonDataContext);

                NewJobNumber = number;
            }
            catch (ArgumentOutOfRangeException exa)
            {
                NoticeWindow.ShowError("Invalid job number", exa.Message);

                return false;
            }
            catch (InvalidOperationException exi)
            {
                NoticeWindow.ShowError("Duplicate job number", exi.Message);

                return false;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Input error", ex.Message);

                return false;
            }

            return true;
        }

        private void BeginAcceptingNumber()
        {
            txtNewJobNumber.Background = Brushes.White;
            txtNewJobNumber.Foreground = Brushes.Black;
        }

        private void InvalidateNumber()
        {
            txtNewJobNumber.Background = Brushes.OrangeRed;
            txtNewJobNumber.Foreground = Brushes.White;
        }
    }
}
