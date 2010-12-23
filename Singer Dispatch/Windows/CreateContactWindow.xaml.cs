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
using System.Collections.ObjectModel;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CreateContactWindow.xaml
    /// </summary>
    public partial class CreateContactWindow : Window
    {
        private SingerDispatchDataContext _database;
        private Contact _contact;
        private Boolean _created;

        public CreateContactWindow(SingerDispatchDataContext database, Company company, Company careOfCompany)
        {
            InitializeComponent();

            _database = database;
            _created = false;
            _contact = new Contact();            

            var list = new List<Company>();

            list.Add(company);

            if (careOfCompany != null)
                list.Add(careOfCompany);

            cmbCompanies.ItemsSource = list;

            _contact.Company = list.First();

            DataContext = _contact;
        }

        public Contact CreateContact()
        {
            ShowDialog();

            return _contact;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbContactTypes.MaxHeight = lbContactTypes.ActualHeight;

            cmbContactPreferedContactMethod.ItemsSource = from cm in _database.ContactMethods select cm;

            var list = new ObservableCollection<CheckBox>();

            foreach (var type in from ct in _database.ContactTypes select ct)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = false };

                cb.Checked += ContactTypeCheckBox_Checked;
                cb.Unchecked += ContactTypeCheckBox_Unchecked;

                list.Add(cb);
            }

            lbContactTypes.ItemsSource = list;
        }

        private void ContactTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var type = (ContactType)cb.DataContext;

            var role = new ContactRoles { Contact = _contact, ContactType = type };

            _contact.ContactRoles.Add(role);
        }

        private void ContactTypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var type = (ContactType)cb.DataContext;

            foreach (var item in (from role in _contact.ContactRoles where role.ContactType == type select role).ToList())
            {
                _contact.ContactRoles.Remove(item);
            }
        }

        private void CreateContact_Click(object sender, RoutedEventArgs e)
        {
            _created = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_created)
                _contact = null;
        }
    }
}
