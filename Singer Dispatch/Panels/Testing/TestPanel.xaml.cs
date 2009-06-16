using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Testing
{
    /// <summary>
    /// Interaction logic for TestPanel.xaml
    /// </summary>
    public partial class TestPanel : UserControl
    {        
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(TestPanel), new PropertyMetadata(null, TestPanel.SelectedCompanyPropertyChanged));

        public TestPanel()
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
            Company company = (Company)e.NewValue;

            if (company != null)
            {
                MessageBox.Show(company.Name);
            }
            else
            {
                MessageBox.Show("NULL");
            }
        }
    }
}
