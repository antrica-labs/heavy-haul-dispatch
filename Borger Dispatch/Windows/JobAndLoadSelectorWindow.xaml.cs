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
using System.Windows.Shapes;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for JobAndLoadSelectorWindow.xaml
    /// </summary>
    public partial class JobAndLoadSelectorWindow : Window
    {
        private SingerDispatchDataContext database;
        private Company company;

        private bool submitted = false;

        public JobAndLoadSelectorWindow(Company company, SingerDispatchDataContext database)
        {
            InitializeComponent();

            this.company = company;
            this.database = database;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbJobs.ItemsSource = (from j in database.Jobs where j.Company == company orderby j.Number descending select j).ToList();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;                
            }
        }

        private void cmbJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbJobs.SelectedItem == null)
                btnSubmit.Content = "Skip this step";
            else
                btnSubmit.Content = "Select job";
        }

        private void cmbLoads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLoads.SelectedItem != null)
                btnSubmit.Content = "Select load";
            else
                btnSubmit.Content = (cmbJobs.SelectedItem == null) ? "Skip this step" : "Select job";
        }

        public JobAndLoadReference GetJobAndLoad(string heading)
        {
            lblHeading.Content = heading;

            return GetJobAndLoad();
        }

        public JobAndLoadReference GetJobAndLoad()
        {
            var reference = new JobAndLoadReference();

            submitted = false;            
            
            ShowDialog();

            reference.Job = cmbJobs.SelectedItem as Job;
            reference.Load = cmbLoads.SelectedItem as Load;

            if (submitted)
                return reference;
            else
                return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JobAndLoadSelected();            
        }

        private void JobAndLoadSelected()
        {
            submitted = true;
            Close();
        }
    }

    public class JobAndLoadReference
    {
        public Job Job { get; set; }
        public Load Load { get; set; }
    }
}
