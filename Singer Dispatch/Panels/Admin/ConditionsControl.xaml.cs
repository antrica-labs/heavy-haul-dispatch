using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;

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

            Database = SingerConstants.CommonDataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TheGrid.ItemsSource = new ObservableCollection<Condition>(from c in Database.Conditions orderby c.ID select c);
        }

        private void RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                try
                {
                    Database.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                    Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
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

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {                
                Database.Conditions.DeleteOnSubmit(condition);
                Database.SubmitChanges();

                list.Remove(condition);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void TheGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
