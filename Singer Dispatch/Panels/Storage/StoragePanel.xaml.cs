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

            dgStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from si in Database.StorageItems where si.DateRemoved == null || si.DateRemoved > DateTime.Now select si);
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
//            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }
    }
}
