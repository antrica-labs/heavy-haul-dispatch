using System.Windows;

namespace SingerDispatch.Panels
{
    public class QuoteUserControl : CompanyUserControl
    {       
        public static DependencyProperty SelectedQuoteProperty = DependencyProperty.Register("SelectedQuote", typeof(Quote), typeof(QuoteUserControl), new PropertyMetadata(null, SelectedQuotePropertyChanged));

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
