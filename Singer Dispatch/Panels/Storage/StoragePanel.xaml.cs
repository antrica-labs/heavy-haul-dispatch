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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SingerDispatch.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StoragePanel.xaml
    /// </summary>
    public partial class StoragePanel 
    {
        public SingerDispatchDataContext Database { get; set; }

        private CommandBinding SaveCommand { get; set; }

        public StoragePanel()
        {
            InitializeComponent();

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;
        }
        
       
        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitJobChanges_Executed;

            if (InDesignMode()) return;

            dgStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from si in Database.StorageItems where si.IsVisible == true select si);
        }

        
        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {

            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = new StorageItem { DateEntered = DateTime.Now };

            list.Add(item);
            Database.StorageItems.InsertOnSubmit(item);

            dgStorageItems.ScrollIntoView(item);
            dgStorageItems.SelectedItem = item;
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (item == null) return;

            item.DateRemoved = item.DateRemoved ?? DateTime.Now;
            item.IsVisible = false;

            list.Remove(item);

            try
            {                
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            try
            {
                button.Focus();

                Database.SubmitChanges();

                button.IsEnabled = false;
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            button.IsEnabled = true;
        }
    }
}
