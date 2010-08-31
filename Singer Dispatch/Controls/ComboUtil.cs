using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace SingerDispatch.Controls
{
    public static class ComboUtil
    {
        private static readonly CommandBinding DeleteCommandBinding = new CommandBinding(ApplicationCommands.Delete, HandleExecuteDeleteCommand);

        private static void HandleExecuteDeleteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var combo = e.Source as ComboBox;

            if (combo != null)
            {
                combo.SelectedItem = null;
                combo.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                combo.Focus();
            }
        }

        #region AllowNull Property

        public static bool GetAllowNull(ComboBox combo)
        {
            if (combo == null)
                throw new ArgumentNullException("combo");

            return (bool)combo.GetValue(AllowNullProperty);
        }

        public static void SetAllowNull(ComboBox combo, bool value)
        {
            if (combo == null)
                throw new ArgumentNullException("combo");

            combo.SetValue(AllowNullProperty, value);
        }

        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.RegisterAttached(
                "AllowNull",
                typeof(bool),
                typeof(ComboUtil),
                new UIPropertyMetadata(HandleAllowNullPropertyChanged));

        private static void HandleAllowNullPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var combo = (ComboBox)o;
            if (true.Equals(e.NewValue))
            {
                if (!combo.CommandBindings.Contains(DeleteCommandBinding))
                    combo.CommandBindings.Add(DeleteCommandBinding);
            }
            else
            {
                combo.CommandBindings.Remove(DeleteCommandBinding);
            }
        }

        #endregion
    }
}


// Controls:ComboUtil.AllowNull="True"