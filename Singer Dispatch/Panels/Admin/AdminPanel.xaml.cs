using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel
    {
        public AdminPanel()
        {
            InitializeComponent();
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
