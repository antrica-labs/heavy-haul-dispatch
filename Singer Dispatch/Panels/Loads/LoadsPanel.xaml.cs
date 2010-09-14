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

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadsPanel.xaml
    /// </summary>
    public partial class LoadsPanel
    {
        public static DependencyProperty SelectedJobProperty = DependencyProperty.Register("SelectedJob", typeof(Job), typeof(LoadsPanel));

        public Job SelectedJob
        {
            set
            {
                SetValue(SelectedJobProperty, value);
            }
            get
            {
                return (Job)GetValue(SelectedJobProperty);
            }
        }
        
        public LoadsPanel()
        {
            InitializeComponent();
        }

        private void ThePanel_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
