using System.Windows.Controls;
using System.Windows;

namespace SingerDispatch.Panels
{
    public class CompanyUserControl : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CompanyUserControl), new PropertyMetadata(null, CompanyUserControl.SelectedCompanyPropertyChanged));

        public Company SelectedCompany
        {
            get
            {
                return (Company)GetValue(SelectedCompanyProperty);
            }
            set
            {
                SetValue(SelectedCompanyProperty, value);
            }
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompanyUserControl control = (CompanyUserControl)d;

            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);
        }

        protected virtual void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
        }
    }
}
