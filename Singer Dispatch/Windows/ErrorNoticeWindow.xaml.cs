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
    /// Interaction logic for ErrorNoticeWindow.xaml
    /// </summary>
    public partial class ErrorNoticeWindow : Window
    {        
        public string Message { get; set; }
        

        public ErrorNoticeWindow(string title, string message)
        {
            this.Title = title;
            this.Message = message;

            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
