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
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoiceExtrasControl.xaml
    /// </summary>
    public partial class InvoiceExtrasControl :InvoiceUserControl
    {
        public InvoiceExtrasControl()
        {
            InitializeComponent();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            dgExtras.ItemsSource = (newValue == null) ? null : new ObservableCollection<InvoiceExtra>(newValue.InvoiceExtras);
        }

        private void NewExtra_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceExtra>)dgExtras.ItemsSource;
            var item = new InvoiceExtra();

            SelectedInvoice.InvoiceExtras.Add(item);
            list.Add(item);
            dgExtras.SelectedItem = item;
            dgExtras.ScrollIntoView(item);

            txtDescription.Focus();
        }

        private void RemoveExtra_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceExtra>)dgExtras.ItemsSource;
            var item = (InvoiceExtra)dgExtras.SelectedItem;

            if (item == null) return;

            list.Remove(item);
            SelectedInvoice.InvoiceExtras.Remove(item);
        }
    }
}
