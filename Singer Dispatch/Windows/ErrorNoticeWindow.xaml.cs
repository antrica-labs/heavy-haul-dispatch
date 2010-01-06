using System.Windows;

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
