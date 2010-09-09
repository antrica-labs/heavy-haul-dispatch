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
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteNotesControl.xaml
    /// </summary>
    public partial class QuoteNotesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteNotesControl()
        {
            InitializeComponent();

            Database = SingerConfigs.CommonDataContext;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);
            
            var quote = (Quote)newValue;

            dgNotes.ItemsSource = (quote == null) ? null : new ObservableCollection<QuoteNote>(quote.QuoteNotes);
        }

        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteNote>)dgNotes.ItemsSource;
            var note = new QuoteNote { Quote = SelectedQuote };

            SelectedQuote.QuoteNotes.Add(note);
            list.Add(note);

            dgNotes.ScrollIntoView(note);
            dgNotes.SelectedItem = note;

            txtNote.Focus();
        }

        private void DuplicateNote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteNote>)dgNotes.ItemsSource;
            var note = (QuoteNote)dgNotes.SelectedItem;

            if (note == null) return;

            var copy = note.Duplicate();

            copy.Quote = SelectedQuote;
            SelectedQuote.QuoteNotes.Add(copy);
            list.Add(copy);

            dgNotes.ScrollIntoView(copy);
            dgNotes.SelectedItem = copy;
        }

        private void RemoveNote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteNote>)dgNotes.ItemsSource;
            var note = (QuoteNote)dgNotes.SelectedItem;

            if (note == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedQuote.QuoteNotes.Remove(note);
            list.Remove(note);
        }
    }
}
