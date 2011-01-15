using System.Windows;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for NoticeWindow.xaml
    /// </summary>
    public partial class NoticeWindow
    {
        public string Heading { get; set; }
        public string Message { get; set; }
        
        public NoticeWindow(string heading, string message)
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
            var window = new NoticeWindow(heading, message);
            window.ShowDialog();
        }

        public static void ShowMessage(string heading, string message)
        {
            var window = new NoticeWindow(heading, message);
            window.ShowDialog();
        }
    }
}
