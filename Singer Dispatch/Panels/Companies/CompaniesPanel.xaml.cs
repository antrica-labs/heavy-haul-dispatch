using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CompaniesPanel.xaml
    /// </summary>
    public partial class CompaniesPanel : CompanyUserControl
    {
        public CompaniesPanel()
        {
            InitializeComponent();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            if (newValue == null)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }
        }
    }
}
