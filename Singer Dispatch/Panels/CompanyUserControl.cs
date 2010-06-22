using System;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels
{
    public class CompanyUserControl : BaseUserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CompanyUserControl), new PropertyMetadata(null, SelectedCompanyPropertyChanged));

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
            var control = (CompanyUserControl)d;

            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);
        }

        protected virtual void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            
        }
    }
}
