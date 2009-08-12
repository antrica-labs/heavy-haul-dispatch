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

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for CommodityMovementControl.xaml
    /// </summary>
    public partial class CommodityMovementControl : JobUserControl
    {
        SingerDispatchDataContext database;

        public CommodityMovementControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {
                cmbCommodityName.ItemsSource = (from c in database.Commodities where c.Company == SelectedJob.Company || c.Company == SelectedJob.Company1 select c).ToList();
            }
            else
            {
                cmbCommodityName.ItemsSource = null;
            }
        }

        private void btnNewCommodity_Click(object sender, RoutedEventArgs e)
        {
            JobCommodity commodity = new JobCommodity() { JobID = SelectedJob.ID };
            
        }
    }
}
