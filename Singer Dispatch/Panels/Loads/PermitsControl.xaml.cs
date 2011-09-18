using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for PermitsControl.xaml
    /// </summary>
    public partial class PermitsControl
    {
        public static DependencyProperty SelectedPermitProperty = DependencyProperty.Register("SelectedPermit", typeof(Permit), typeof(PermitsControl), new PropertyMetadata(null, SelectedPermitPropertyChanged));

        public Permit SelectedPermit
        {
            set
            {
                SetValue(SelectedPermitProperty, value);
            }
            get
            {
                return GetValue(SelectedPermitProperty) as Permit;
            }
        }

        public PermitsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            cmbCompanies.ItemsSource = from s in Database.Services where s.ServiceType == (from st in Database.ServiceTypes where st.Name == "Permits" select st).First() orderby s.Company.Name select s.Company;
            cmbPermitTypes.ItemsSource = (SelectedLoad == null) ? null : from pt in Database.PermitTypes select pt;            
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            dgPermits.ItemsSource = (newValue == null) ? null : new ObservableCollection<Permit>(newValue.Permits);
        }

        public static void SelectedPermitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PermitsControl)d;

            control.dgPermits.SelectedItem = (Permit)e.NewValue;
        }

        private void NewPermit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var list = (ObservableCollection<Permit>)dgPermits.ItemsSource;
            var permit = new Permit { LoadID = SelectedLoad.ID };

            SelectedLoad.Permits.Add(permit);
            list.Add(permit);
            dgPermits.SelectedItem = permit;
            dgPermits.ScrollIntoView(permit);

            cmbCompanies.Focus();
        }

        private void DuplicatePermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = (Permit)dgPermits.SelectedItem;
            var list = (ObservableCollection<Permit>)dgPermits.ItemsSource;

            if (permit == null)
                return;

            permit = permit.Duplicate();

            SelectedLoad.Permits.Add(permit);
            list.Add(permit);
            dgPermits.SelectedItem = permit;
            dgPermits.ScrollIntoView(permit);                

        }

        private void RemovePermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = (Permit)dgPermits.SelectedItem;

            if (permit == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedLoad.Permits.Remove(permit);
            ((ObservableCollection<Permit>)dgPermits.ItemsSource).Remove(permit);

            dgPermits.SelectedItem = null;
        }
    }
}
