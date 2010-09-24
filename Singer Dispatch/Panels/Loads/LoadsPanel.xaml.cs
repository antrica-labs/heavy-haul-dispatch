﻿using System;
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
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public static DependencyProperty SelectedJobProperty = DependencyProperty.Register("SelectedJob", typeof(Job), typeof(LoadsPanel), new PropertyMetadata(null, SelectedJobPropertyChanged));

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

        public LoadsPanel()
        {
            InitializeComponent();

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitQuoteChanges_Executed;

            if (InDesignMode()) return;

            UpdateLoadList();
        }

        public static void SelectedJobPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LoadsPanel)d;

            control.SelectedJobChanged((Job)e.NewValue, (Job)e.OldValue);
        }

        protected void SelectedJobChanged(Job newValue, Job oldValue)
        {            
            UpdateLoadList();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            cmbJobList.ItemsSource = from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j;
        }

        protected override void UseImperialMeasurementsChanged(bool value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        public void UpdateLoadList()
        {
            dgLoads.ItemsSource = (SelectedJob == null) ? null : new ObservableCollection<Load>(from l in Database.Loads where l.Job == SelectedJob orderby l.Number descending select l);
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;
            var load = new Load { Job = SelectedJob, Status = SelectedJob.Status };

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
                ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void DuplicateLoad_Click(object sender, RoutedEventArgs e)
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
                ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
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
                EntityHelper.PrepareEntityDelete(load, Database);

                list.Remove(load);
                SelectedJob.Loads.Remove(load);
                dgLoads.SelectedItem = null;
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting to delete load", ex.Message);
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
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-SingerPrintFileCopy") ?? "false");
            }

            var viewer = new DocumentViewerWindow(new DispatchDocument(printFileCopy), dispatches, string.Format("Dispatches - Load #{0}-{1}", SelectedJob.Number, SelectedLoad.Number)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void CommitQuoteChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void CommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var button = (ButtonBase)sender;

            try
            {
                button.Focus();

                Database.SubmitChanges();

                button.IsEnabled = false;
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            button.IsEnabled = true;
        }

        private void EditJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewJob(SelectedJob);
        }
    }
}
