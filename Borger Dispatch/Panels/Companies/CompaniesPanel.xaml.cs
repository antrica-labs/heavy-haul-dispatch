using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CompaniesPanel.xaml
    /// </summary>
    public partial class CompaniesPanel
    {
        public CompaniesPanel()
        {
            InitializeComponent();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
        }

        protected override void UseImperialMeasurementsChanged(bool value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        protected override void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {
            base.CompanyListChanged(newValue, oldValue);
        }
    }
}
