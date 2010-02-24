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
    }
}
