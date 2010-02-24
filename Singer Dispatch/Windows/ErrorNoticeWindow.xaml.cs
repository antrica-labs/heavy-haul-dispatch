using System.Windows;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for ErrorNoticeWindow.xaml
    /// </summary>
    public partial class ErrorNoticeWindow
    {
        public string Heading { get; set; }
        public string Message { get; set; }
        
        public ErrorNoticeWindow(string heading, string message)
        {
            Heading = heading;
            Message = message;

            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void ShowError(string heading, string message)
        {
            var window = new ErrorNoticeWindow(heading, message);
            window.ShowDialog();
        }
    }
}
