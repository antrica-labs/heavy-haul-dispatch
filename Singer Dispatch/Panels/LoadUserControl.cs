﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SingerDispatch.Panels
{
    public class LoadUserControl : CompanyUserControl
    {
        public static DependencyProperty SelectedLoadProperty = DependencyProperty.Register("SelectedLoad", typeof(Load), typeof(LoadUserControl), new PropertyMetadata(null, SelectedLoadPropertyChanged));

        public Job SelectedLoad
        {
            set
            {
                SetValue(SelectedLoadProperty, value);
            }
            get
            {
                return (Job)GetValue(SelectedLoadProperty);
            }
        }

        public static void SelectedLoadPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LoadUserControl)d;

            control.SelectedLoadChanged((Load)e.NewValue, (Load)e.OldValue);
        }

        protected virtual void SelectedLoadChanged(Load newValue, Load oldValue)
        {
        }
    }
}
