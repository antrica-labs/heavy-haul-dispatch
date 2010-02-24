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

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for InclusionsControl.xaml
    /// </summary>
    public partial class InclusionsControl : UserControl
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
                catch (System.Exception ex)
                {
                    SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }

        private void NewInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var Inclusion = new Inclusion();

            Database.Inclusions.InsertOnSubmit(Inclusion);
            list.Add(Inclusion);
            TheGrid.SelectedItem = Inclusion;
            TheGrid.ScrollIntoView(Inclusion);

            DataGridHelper.GetCell(TheGrid, TheGrid.SelectedIndex, 0).Focus();
        }

        private void RemoveInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var Inclusion = (Inclusion)TheGrid.SelectedItem;

            if (Inclusion == null) return;

            try
            {
                list.Remove(Inclusion);
                Database.Inclusions.DeleteOnSubmit(Inclusion);

                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
