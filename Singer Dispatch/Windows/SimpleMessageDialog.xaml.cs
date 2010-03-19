using System.Windows;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for SimpleMessageDialog.xaml
    /// </summary>
    public partial class SimpleMessageDialog : Window
    {
        public SimpleMessageDialog(string message)
        {
            InitializeComponent();

            TheMessage.Content = message;
        }
    }
}
