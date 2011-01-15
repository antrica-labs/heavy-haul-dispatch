using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for ConditionsControl.xaml
    /// </summary>
    public partial class ConditionsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public ConditionsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            TheGrid.ItemsSource = new ObservableCollection<Condition>(from c in Database.Conditions where c.Archived != true orderby c.ID select c);
        }

        private void NewCondition_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Condition>)TheGrid.ItemsSource;
            var condition = new Condition();

            Database.Conditions.InsertOnSubmit(condition);
            list.Add(condition);

            DataGridHelper.EditFirstColumn(TheGrid, condition);
        }

        private void RemoveCondition_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Condition>)TheGrid.ItemsSource;
            var condition = (Condition)TheGrid.SelectedItem;

            if (condition == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                condition.Archived = true;
                
                Database.SubmitChanges();

                list.Remove(condition);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChanges()
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                CommitChanges();
            }
        }
        
    }
}
