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

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            TheGrid.ItemsSource = new ObservableCollection<Inclusion>(from i in Database.Inclusions where i.Archived != null orderby i.Line select i);
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
                inclusion.Archived = true;

                Database.SubmitChanges();

                list.Remove(inclusion);
            }
            catch (Exception ex)
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
