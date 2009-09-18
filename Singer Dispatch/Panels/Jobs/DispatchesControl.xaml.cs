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
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for DispatchesControl.xaml
    /// </summary>
    public partial class DispatchesControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public DispatchesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;            
            cmbServiceTypes.ItemsSource = SingerConstants.ServiceTypes;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = SelectedJob != null ? SelectedJob.Loads : null;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            if (newValue != null)
            {
                dgDispatches.ItemsSource = new ObservableCollection<Dispatch>((from d in Database.Dispatches where d.Job == SelectedJob select d).ToList());
            }
            else
            {
                dgDispatches.ItemsSource = null;
            }
        }

        private void btnNewDispatch_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = new Dispatch { JobID = SelectedJob.ID };

            SelectedJob.Dispatches.Add(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Insert(0, dispatch);
            dgDispatches.SelectedItem = dispatch;

            cmbLoads.Focus();
        }
       
    }
}
