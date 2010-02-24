using System;
using System.Linq;
using System.Windows;
using SingerDispatch.Controls;
using System.Collections.ObjectModel;

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

            Database = SingerConstants.CommonDataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TheGrid.ItemsSource = new ObservableCollection<Inclusion>(from i in Database.Inclusions orderby i.Line select i);
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == Microsoft.Windows.Controls.DataGridEditAction.Commit)
            {
                try
                {
                    Database.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }

        private void NewInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = new Inclusion();

            Database.Inclusions.InsertOnSubmit(inclusion);
            list.Add(inclusion);
            TheGrid.SelectedItem = inclusion;
            TheGrid.ScrollIntoView(inclusion);

            DataGridHelper.GetCell(TheGrid, TheGrid.SelectedIndex, 0).Focus();
        }

        private void RemoveInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = (Inclusion)TheGrid.SelectedItem;

            if (inclusion == null) return;

            try
            {
                list.Remove(inclusion);
                Database.Inclusions.DeleteOnSubmit(inclusion);

                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
