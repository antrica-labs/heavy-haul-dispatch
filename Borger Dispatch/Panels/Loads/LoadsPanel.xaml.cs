using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadsPanel.xaml
    /// </summary>
    public partial class LoadsPanel
    {
        public static DependencyProperty SelectedJobProperty = DependencyProperty.Register("SelectedJob", typeof(Job), typeof(LoadsPanel), new PropertyMetadata(null, SelectedJobPropertyChanged));
        public static DependencyProperty SelectedPermitProperty = DependencyProperty.Register("SelectedPermit", typeof(Permit), typeof(LoadsPanel), new PropertyMetadata(null, SelectedPermitPropertyChanged));
        public static DependencyProperty SelectedThirdPartyServiceProperty = DependencyProperty.Register("SelectedThirdPartyService", typeof(ThirdPartyService), typeof(LoadsPanel), new PropertyMetadata(null, SelectedThirdPartyServicePropertyChanged));

        public Job SelectedJob
        {
            set
            {
                SetValue(SelectedJobProperty, value);
            }
            get
            {
                return (Job)GetValue(SelectedJobProperty);
            }
        }

        public Permit SelectedPermit
        {
            set
            {
                SetValue(SelectedPermitProperty, value);
            }
            get
            {
                return GetValue(SelectedPermitProperty) as Permit;
            }
        }

        public ThirdPartyService SelectedThirdPartyService
        {
            set
            {
                SetValue(SelectedThirdPartyServiceProperty, value);
            }
            get
            {
                return GetValue(SelectedThirdPartyServiceProperty) as ThirdPartyService;
            }
        }

        public LoadsPanel()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateJobList();
            UpdateLoadList();
        }

        protected override void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {
            base.CompanyListChanged(newValue, oldValue);
        }

        public static void SelectedJobPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LoadsPanel)d;

            control.SelectedJobChanged((Job)e.NewValue, (Job)e.OldValue);
        }

        public static void SelectedPermitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LoadsPanel)d;

            control.Tabs.SelectedIndex = 4;
        }

        public static void SelectedThirdPartyServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LoadsPanel)d;

            control.Tabs.SelectedIndex = 3;
        }

        protected void SelectedJobChanged(Job newValue, Job oldValue)
        {            
            UpdateLoadList();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            UpdateJobList();
        }

        protected override void UseImperialMeasurementsChanged(bool value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        private void UpdateJobList()
        {
            cmbJobList.ItemsSource = (SelectedCompany == null) ? null : from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j;
        }

        private void UpdateLoadList()
        {
            dgLoads.ItemsSource = (SelectedJob == null) ? null : new ObservableCollection<Load>(from l in Database.Loads where l.Job == SelectedJob orderby l.Number descending select l);
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;
            var load = new Load { Job = SelectedJob };

            try
            {
                load.Status = (from s in Database.Statuses where s.Name == "In Process" select s).First();
            }
            catch
            { }            

            foreach (var item in SelectedJob.ReferenceNumbers)
            {
                var reference = new LoadReferenceNumber { Field = item.Field, Value = item.Value };

                load.ReferenceNumbers.Add(reference);
            }

            SelectedJob.Loads.Add(load);
            list.Insert(0, load);

            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CopyLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;

            if (load == null)
                return;

            load = load.Duplicate();

            SelectedJob.Loads.Add(load);
            list.Insert(0, load);

            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void DeleteLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;

            if (load == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                Database.SubmitChanges(); // Save any unsaved changes before doing delete.

                EntityHelper.PrepareEntityDelete(load, Database);
                                
                SelectedJob.Loads.Remove(load);
                Database.SubmitChanges();

                list.Remove(load);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to delete load", ex.Message);

                Database.RevertChanges();
            }
        }

        private void ViewAllDispatches_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;
             
            var dispatches = (from d in SelectedLoad.Dispatches select d).ToList();

            bool printFileCopy;

            if (SelectedCompany.CustomerType.IsEnterprise == true)
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-EnterprisePrintFileCopy") ?? "false");
            }
            else
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-CompanyPrintFileCopy") ?? "false");
            }

            var viewer = new DocumentViewerWindow(new DispatchDocument(printFileCopy), dispatches, string.Format("Dispatches - Load #{0}-{1}", SelectedJob.Number, SelectedLoad.Number)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void EditJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewJob(SelectedJob);
        }
    }
}
