using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch
{
    partial class SingerDispatchDictionary
    {
        private bool ChangesMade;
        
        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void InputControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!ChangesMade) return;

            if (sender is TextBox)
            {
                var expr = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);

                if (expr != null)
                    expr.UpdateSource();
                else
                    // This textbox is not attached to an entity, so no need to save changes made to it
                    return;
            }

            try
            {
                SingerConfigs.CommonDataContext.SubmitChanges();
                ChangesMade = false;
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangesMade = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangesMade = true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangesMade = true;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangesMade = true;
        }
    }
}
