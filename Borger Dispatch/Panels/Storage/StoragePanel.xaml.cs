using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SingerDispatch.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StoragePanel.xaml
    /// </summary>
    public partial class StoragePanel 
    {
        public StoragePanel()
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
