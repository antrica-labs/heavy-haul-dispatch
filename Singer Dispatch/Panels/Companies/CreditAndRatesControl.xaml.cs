using System.Windows.Controls;
using System.Windows;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CreditAndRatesControl), new PropertyMetadata(null, CreditAndRatesControl.SelectedCompanyPropertyChanged));

        private SingerDispatchDataContext database;

        public CreditAndRatesControl()
        {
            InitializeComponent();

            database = new SingerDispatchDataContext();
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

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {

        }

        
    }
}
