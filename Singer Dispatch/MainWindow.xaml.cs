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

            BuildMainNavigation();
        }

        private void BuildMainNavigation()
        {
            addLinksToExpander(linksCompanies, tabCompanies.Items);
            addLinksToExpander(linksQuotes, tabQuotes.Items);
        }

        private void addLinksToExpander(StackPanel panel, ItemCollection tabs)
        {
            foreach (TabItem tab in tabs)
            {
                TabIndexHyperlink link = new TabIndexHyperlink(tab);
                link.Click += new RoutedEventHandler(SwitchTabs);

                Label label = new Label();
                label.Content = link;

                panel.Children.Add(label);
            }
        }

        private void SwitchTabs(object sender, RoutedEventArgs e)
        {
            TabIndexHyperlink link = (TabIndexHyperlink)e.Source;

            link.Tab.IsSelected = true;

        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }    
    }
}
