using System;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels
{
    public class BaseUserControl : UserControl
    {
        public static DependencyProperty UseImperialMeasurementsProperty = DependencyProperty.Register("UseImperialMeasurements", typeof(Boolean), typeof(BaseUserControl), new PropertyMetadata(null, UseImperialMeasurementsPropertyChanged));

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

        public static void UseImperialMeasurementsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaseUserControl)d;

            control.UseImperialMeasurementsChanged((Boolean)e.NewValue);
        }

        protected virtual void UseImperialMeasurementsChanged(Boolean value)
        {

        }
    }
}
