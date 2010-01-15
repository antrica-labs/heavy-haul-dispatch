using System.Windows.Input;

namespace SingerDispatch.Controls
{
    public static class CustomCommands
    {
        public static RoutedCommand QuoteLoookupCommand = new RoutedCommand();
        public static RoutedCommand JobLookupCommand = new RoutedCommand();
        public static RoutedCommand InvoiceLookupCommand = new RoutedCommand();
        public static RoutedCommand CreateCompanyCommand = new RoutedCommand();
        public static RoutedCommand EditCompaniesCommand = new RoutedCommand();

        public static RoutedCommand GenericSaveCommand = new RoutedCommand();

        static CustomCommands()
        {
            QuoteLoookupCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Shift | ModifierKeys.Control));
            JobLookupCommand.InputGestures.Add(new KeyGesture(Key.J, ModifierKeys.Shift | ModifierKeys.Control));
            InvoiceLookupCommand.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Shift | ModifierKeys.Control));
            CreateCompanyCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Shift | ModifierKeys.Control));
            EditCompaniesCommand.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Shift | ModifierKeys.Control));

            GenericSaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
        }
    }
}
