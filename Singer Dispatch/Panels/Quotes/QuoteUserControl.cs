using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace SingerDispatch.Panels.Quotes
{
    public class QuoteUserControl : UserControl
    {       
        public static DependencyProperty SelectedQuoteProperty = DependencyProperty.Register("SelectedQuote", typeof(Quote), typeof(QuoteUserControl), new PropertyMetadata(null, QuoteUserControl.SelectedQuotePropertyChanged));

        public Quote SelectedQuote
        {
            get
            {
                return (Quote)GetValue(SelectedQuoteProperty);
            }
            set
            {
                SetValue(SelectedQuoteProperty, value);
            }
        }

        public static void SelectedQuotePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (QuoteUserControl)d;

            control.SelectedQuoteChanged((Quote)e.NewValue, (Quote)e.OldValue);
        }

        protected virtual void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
        }
    }
}
