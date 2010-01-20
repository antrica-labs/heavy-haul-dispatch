﻿using System.Windows;

namespace SingerDispatch.Panels
{
    public class InvoiceUserControl : JobUserControl
    {
        public static DependencyProperty SelectedInvoiceProperty = DependencyProperty.Register("SelectedInvoice", typeof(Invoice), typeof(InvoiceUserControl), new PropertyMetadata(null, InvoiceUserControl.SelectedInvoicePropertyChanged));

        public Invoice SelectedInvoice
        {
            get
            {
                return (Invoice)GetValue(SelectedInvoiceProperty);
            }

            set
            {
                SetValue(SelectedInvoiceProperty, value);
            }
        }

        public static void SelectedInvoicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvoiceUserControl control = (InvoiceUserControl)d;

            control.SelectedInvoiceChanged((Invoice)e.NewValue, (Invoice)e.OldValue);
        }

        protected virtual void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
        }
    }
}
