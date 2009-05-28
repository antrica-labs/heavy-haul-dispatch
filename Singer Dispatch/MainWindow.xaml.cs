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
using System.Windows.Shapes;

namespace Singer_Dispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnViewContacts_Click(object sender, RoutedEventArgs e)
        {
            tabContacts.SelectedIndex = 0;
        }

        private void btnViewContactsCommodities_Click(object sender, RoutedEventArgs e)
        {
            tabContacts.SelectedIndex = 1;
        }

        private void btnViewContactsCredit_Click(object sender, RoutedEventArgs e)
        {
            tabContacts.SelectedIndex = 2;
        }

    }
}
