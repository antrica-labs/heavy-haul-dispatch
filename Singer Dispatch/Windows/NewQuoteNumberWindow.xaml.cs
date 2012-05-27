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
    /// Interaction logic for NewQuoteNumberWindows.xaml
    /// </summary>
    public partial class NewQuoteNumberWindow : Window
    {
        private string NewQuoteNumber { get; set; }

        public NewQuoteNumberWindow()
        {
            InitializeComponent();

            NewQuoteNumber = null;
        }

        public string CreateQuoteNumber()
        {
            ShowDialog();

            return NewQuoteNumber;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    PickQuoteNumber();
                    break;
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            PickQuoteNumber();
        }

        private void txtNewQuoteNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            BeginAcceptingNumber();
        }

        private void PickQuoteNumber()
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
                var number = txtNewQuoteNumber.Text;

                EntityHelper.SuggestQuoteNumber(number, SingerConfigs.CommonDataContext);

                NewQuoteNumber = number;
            }
            catch (ArgumentOutOfRangeException exa)
            {
                NoticeWindow.ShowError("Invalid quote number", exa.Message);

                return false;
            }
            catch (InvalidOperationException exi)
            {
                NoticeWindow.ShowError("Duplicate quote number", exi.Message);

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
            txtNewQuoteNumber.Background = Brushes.White;
            txtNewQuoteNumber.Foreground = Brushes.Black;
        }

        private void InvalidateNumber()
        {
            txtNewQuoteNumber.Background = Brushes.OrangeRed;
            txtNewQuoteNumber.Foreground = Brushes.White;
        }
    }
}
