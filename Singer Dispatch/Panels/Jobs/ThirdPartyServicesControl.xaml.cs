﻿using System;
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
    /// Interaction logic for ThirdPartyServicesControl.xaml
    /// </summary>
    public partial class ThirdPartyServicesControl : JobUserControl
    {
        public ThirdPartyServicesControl()
        {
            InitializeComponent();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {
                cmbLoads.ItemsSource = SelectedJob.Loads;
            }
            else
            {
                cmbLoads.ItemsSource = null;
            }
        }
    }
}
