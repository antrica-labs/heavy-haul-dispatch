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
    /// Interaction logic for LoadsControl.xaml
    /// </summary>
    public partial class LoadsControl : JobUserControl
    {
        SingerDispatchDataContext database;

        public LoadsControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {            
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgLoads.ItemsSource = new ObservableCollection<Load>((from l in database.Loads where l.Job == newValue select l).ToList());
        }

        private void btnNewLoad_Click(object sender, RoutedEventArgs e)
        {
            Load load = new Load() { JobID = SelectedJob.ID };

            SelectedJob.Loads.Add(load);
            ((ObservableCollection<Load>)dgLoads.ItemsSource).Insert(0, load);
            dgLoads.SelectedItem = load;

            cmbUnits.Focus();
        }

        
    }
}
