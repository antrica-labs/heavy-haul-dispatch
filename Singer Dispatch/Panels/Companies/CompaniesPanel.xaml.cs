using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CompaniesPanel.xaml
    /// </summary>
    public partial class CompaniesPanel : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CompaniesPanel), new PropertyMetadata(null, CompaniesPanel.SelectedCompanyPropertyChanged));

        public CompaniesPanel()
        {
            InitializeComponent();
        }
        
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
            
        }
    }
}
