using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels
{
    public class BaseUserControl : UserControl
    {

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
