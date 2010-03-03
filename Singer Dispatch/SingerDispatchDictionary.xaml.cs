using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch
{
    partial class SingerDispatchDictionary
    {
        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
