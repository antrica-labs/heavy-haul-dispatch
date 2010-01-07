using System.Windows;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for ErrorNoticeWindow.xaml
    /// </summary>
    public partial class ErrorNoticeWindow : Window
    {
        public string Heading { get; set; }
        public string Message { get; set; }
        
        public ErrorNoticeWindow(string heading, string message)
        {
            this.Heading = heading;
            this.Message = message;

            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void ShowError(string heading, string message)
        {
            var window = new ErrorNoticeWindow(heading, message);
            window.ShowDialog();
        }
    }
}
