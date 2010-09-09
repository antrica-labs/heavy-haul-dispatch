using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for InclusionsControl.xaml
    /// </summary>
    public partial class InclusionsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public InclusionsControl()
        {
            InitializeComponent();

            Database = SingerConfigs.CommonDataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TheGrid.ItemsSource = new ObservableCollection<Inclusion>(from i in Database.Inclusions orderby i.Line select i);
        }

        private void NewInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = new Inclusion();

            Database.Inclusions.InsertOnSubmit(inclusion);
            list.Add(inclusion);

            DataGridHelper.EditFirstColumn(TheGrid, inclusion);
        }

        private void RemoveInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = (Inclusion)TheGrid.SelectedItem;

            if (inclusion == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {                
                Database.Inclusions.DeleteOnSubmit(inclusion);
                Database.SubmitChanges();

                list.Remove(inclusion);
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ButtonBase)sender).Focus();
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
            }
        }
    }
}
