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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

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

        protected override void UseImperialMeasurementsChanged(Boolean value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        protected override void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {
            base.CompanyListChanged(newValue, oldValue);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {            

            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = new StorageItem { DateEntered = DateTime.Now };

            list.Add(item);
            Database.StorageItems.InsertOnSubmit(item);

            dgStorageItems.ScrollIntoView(item);
            dgStorageItems.SelectedItem = item;

            cmbCompanies.Focus();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {

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
