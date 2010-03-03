﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteConditionsControl.xaml
    /// </summary>
    public partial class QuoteConditionsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteConditionsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            TheList.ItemsSource = new ObservableCollection<CheckBox>();
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            list.Clear();

            if (SelectedQuote == null) return;

            var conditions = from c in Database.Conditions select c;

            foreach (var condition in conditions)
            {
                QuoteCondition qc;
                bool isChecked;

                try
                {
                    var copy = condition;
                    qc = (from s in SelectedQuote.QuoteConditions where s.ConditionID == copy.ID select s).First();
                    isChecked = true;
                }
                catch 
                {
                    qc = new QuoteCondition { Line = condition.Line, ConditionID = condition.ID};
                    isChecked = false;
                }

                var cb = new CheckBox { DataContext = qc, IsChecked = isChecked };
                var tb = new TextBox { Style = (Style)TryFindResource("LongText") };
                var binding = new Binding("Line");

                tb.SetBinding(TextBox.TextProperty, binding);

                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                cb.Content = tb;
                
                list.Add(cb);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (QuoteCondition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            if (!SelectedQuote.QuoteConditions.Contains(condition))
                SelectedQuote.QuoteConditions.Add(condition);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var condition = (QuoteCondition)cb.DataContext;

            if (SelectedQuote == null || condition == null) return;

            if (SelectedQuote.QuoteConditions.Contains(condition))
                SelectedQuote.QuoteConditions.Remove(condition);
        }
    }
}
