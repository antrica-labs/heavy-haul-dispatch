﻿using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Jobs
{
    public class JobUserControl : UserControl
    {
        public static DependencyProperty SelectedJobProperty = DependencyProperty.Register("SelectedJob", typeof(Job), typeof(JobUserControl), new PropertyMetadata(null, JobUserControl.SelectedJobPropertyChanged));

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
            JobUserControl control = (JobUserControl)d;

            control.SelectedJobChanged((Job)e.NewValue, (Job)e.OldValue);
        }

        protected virtual void SelectedJobChanged(Job newValue, Job oldValue)
        {
        }
    }
}