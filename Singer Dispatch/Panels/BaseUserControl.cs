using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace SingerDispatch.Panels
{
    public class BaseUserControl : UserControl
    {

        private List<BackgroundWorker> ThreadList;

        public SingerDispatchDataContext Database { get; set; }

        public static DependencyProperty CompanyListProperty = DependencyProperty.Register("CompanyList", typeof(ObservableCollection<Company>), typeof(BaseUserControl), new PropertyMetadata(null, CompanyListPropertyChanged));
        public static DependencyProperty UseImperialMeasurementsProperty = DependencyProperty.Register("UseImperialMeasurements", typeof(Boolean), typeof(BaseUserControl), new PropertyMetadata(false, UseImperialMeasurementsPropertyChanged));

        public Boolean UseImperialMeasurements
        {
            get
            {
                return (Boolean)GetValue(UseImperialMeasurementsProperty);
            }
            set
            {
                SetValue(UseImperialMeasurementsProperty, value);
            }
        }

        public ObservableCollection<Company> CompanyList
        {
            get
            {
                return (ObservableCollection<Company>)GetValue(CompanyListProperty);
            }
            set
            {
                SetValue(CompanyListProperty, value);
            }
        }

        public BaseUserControl()
        {
            ThreadList = new List<BackgroundWorker>();
        }

        public void RegisterThread(BackgroundWorker worker)
        {
            ThreadList.Add(worker);
        }

        public bool CancelThreads()
        {
            var allClear = true;

            foreach (var worker in ThreadList)
            {
                if (worker.WorkerSupportsCancellation)
                    worker.CancelAsync();
                else
                    allClear = false;
            }

            return allClear;
        }

        public static void UseImperialMeasurementsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaseUserControl)d;

            control.UseImperialMeasurementsChanged((Boolean)e.NewValue);
        }

        public static void CompanyListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaseUserControl)d;

            control.CompanyListChanged((ObservableCollection<Company>)e.NewValue, (ObservableCollection<Company>)e.OldValue);
        }

        protected virtual void UseImperialMeasurementsChanged(Boolean value)
        {

        }

        protected virtual void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {

        }

        protected bool InDesignMode()
        {   
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());            
        }
    }
}
