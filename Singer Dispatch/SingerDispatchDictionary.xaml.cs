using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SingerDispatch
{
    partial class SingerDispatchDictionary
    {
        private bool ChangesMade;
        
        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            ChangesMade = false;
        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangesMade = false;
        }

        private void InputControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!ChangesMade) return;

            if (sender is TextBox)
            {
                var tb = sender as TextBox;
                var expr = System.Windows.Data.BindingOperations.GetBindingExpressionBase(tb, TextBox.TextProperty);

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
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", string.Format("{0}\n\n{1}", "It is recommended that you restart the application at this point.", ex.ToString()));
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox)
            {
                var tb = sender as TextBox;
                var expr = System.Windows.Data.BindingOperations.GetBindingExpressionBase(tb, TextBox.TextProperty);

                if (expr != null)
                {
                    expr.UpdateSource();
                    tb.SelectAll();
                }
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

        private void NullableComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var combo = sender as ComboBox;

                combo.SelectedIndex = -1;
                combo.IsDropDownOpen = false;
            }
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
