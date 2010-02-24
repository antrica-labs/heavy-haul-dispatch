using System.Windows;

namespace SingerDispatch.Panels
{
    public class JobUserControl : CompanyUserControl
    {
        public static DependencyProperty SelectedJobProperty = DependencyProperty.Register("SelectedJob", typeof(Job), typeof(JobUserControl), new PropertyMetadata(null, SelectedJobPropertyChanged));

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

        public static void SelectedJobPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (JobUserControl)d;

            control.SelectedJobChanged((Job)e.NewValue, (Job)e.OldValue);
        }

        protected virtual void SelectedJobChanged(Job newValue, Job oldValue)
        {
        }
    }
}
