using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteCommoditiesControl.xaml
    /// </summary>
    public partial class QuoteCommoditiesControl
    {
        public static DependencyProperty CommonSiteNamesProperty = DependencyProperty.Register("CommonSiteNames", typeof(ObservableCollection<string>), typeof(QuoteCommoditiesControl));
        public static DependencyProperty CommonSiteAddressesProperty = DependencyProperty.Register("CommonSiteAddresses", typeof(ObservableCollection<string>), typeof(QuoteCommoditiesControl));

        public SingerDispatchDataContext Database { get; set; }

        public ObservableCollection<string> CommonSiteNames
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteNamesProperty);
            }
            set
            {
                SetValue(CommonSiteNamesProperty, value);
            }
        }

        public ObservableCollection<string> CommonSiteAddresses
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteAddressesProperty);
            }
            set
            {
                SetValue(CommonSiteAddressesProperty, value);
            }
        }

        public QuoteCommoditiesControl()
        {
            InitializeComponent();
            
            CommonSiteNames = new ObservableCollection<string>();
            CommonSiteAddresses = new ObservableCollection<string>();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            if (dgRecordedCommodities.ActualHeight > 0.0)
            {
                dgRecordedCommodities.MaxHeight = dgRecordedCommodities.ActualHeight;
                dgRecordedCommodities.ItemsSource = (SelectedQuote == null) ? null : from c in Database.Commodities where c.Company == SelectedCompany || c.Company == SelectedQuote.CareOfCompany orderby c.Name, c.Unit select c;
            }
            
            UpdateAddressesAndSites();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgQuoteCommodities.ItemsSource = newValue != null ? new ObservableCollection<QuoteCommodity>(from qc in newValue.QuoteCommodities orderby qc.OrderIndex select qc) : null;
        }

        private void dgQuoteCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddressesAndSites();
        }

        private void UpdateAddressesAndSites()
        {
            var list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (!string.IsNullOrWhiteSpace(item.DepartureAddress) && !CommonSiteAddresses.Contains(item.DepartureAddress))
                        CommonSiteAddresses.Add(item.DepartureAddress);
                    if (!string.IsNullOrWhiteSpace(item.ArrivalAddress) && !CommonSiteAddresses.Contains(item.ArrivalAddress))
                        CommonSiteAddresses.Add(item.ArrivalAddress);
                    if (!string.IsNullOrWhiteSpace(item.DepartureSiteName) && !CommonSiteNames.Contains(item.DepartureSiteName))
                        CommonSiteNames.Add(item.DepartureSiteName);
                    if (!string.IsNullOrWhiteSpace(item.ArrivalSiteName) && !CommonSiteNames.Contains(item.ArrivalSiteName))
                        CommonSiteNames.Add(item.ArrivalSiteName);
                }
            }
        }

        private void dgRecordedCommodities_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var qc = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (qc != null)
                qc.OriginalCommodity = null;                
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            UpdateAddressesAndSites();

            var list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;
            var commodity = new QuoteCommodity { QuoteID = SelectedQuote.ID };

            SelectedQuote.QuoteCommodities.Add(commodity);
            list.Add(commodity);            
            dgQuoteCommodities.SelectedItem = commodity;
            dgQuoteCommodities.ScrollIntoView(commodity);

            ReindexCollection(list);

            txtCommodityName.Focus();
        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;
            var list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;

            if (commodity == null)
                return;

            commodity = commodity.Duplicate();
            
            SelectedQuote.QuoteCommodities.Add(commodity);
            list.Add(commodity);
            dgQuoteCommodities.SelectedItem = commodity;
            dgQuoteCommodities.ScrollIntoView(commodity);

            ReindexCollection(list);
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null)
            {
                return;
            }

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            dgQuoteCommodities.SelectedItem = null;
            SelectedQuote.QuoteCommodities.Remove(commodity);
            ((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource).Remove(commodity);

            ReindexCollection((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource);
        }

        private static void ReindexCollection(ObservableCollection<QuoteCommodity> list)
        {
            int index = 1;

            foreach (var item in list)
            {
                item.OrderIndex = index;

                index++;
            }
        }


        #region DraggedItem

        /// <summary>
        /// DraggedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty DraggedItemProperty = DependencyProperty.Register("DraggedItem", typeof(QuoteCommodity), typeof(QuoteCommoditiesControl));

        /// <summary>
        /// Gets or sets the DraggedItem property.  This dependency property 
        /// indicates ....
        /// </summary>
        public QuoteCommodity DraggedItem
        {
            get { return (QuoteCommodity)GetValue(DraggedItemProperty); }
            set { SetValue(DraggedItemProperty, value); }
        }

        #endregion

        #region edit mode monitoring

        /// <summary>
        /// State flag which indicates whether the grid is in edit
        /// mode or not.
        /// </summary>
        public bool IsEditing { get; set; }

        private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            //in case we are in the middle of a drag/drop operation, cancel it...
            if (IsDragging) ResetDragDrop();
        }

        private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

        #endregion

        #region Drag and Drop Rows

        /// <summary>
        /// Keeps in mind whether
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Initiates a drag action if the grid is not in edit mode.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing) return;

            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(dgQuoteCommodities));
            if (row == null || row.IsEditing) return;

            //set flag that indicates we're capturing mouse movements
            IsDragging = true;
            DraggedItem = (QuoteCommodity)row.Item;
        }


        /// <summary>
        /// Completes a drag/drop operation.
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging || IsEditing)
            {
                return;
            }

            //get the target item
            var targetItem = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
            {
                var list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;

                //remove the source from the list
                list.Remove(DraggedItem);

                //get target index
                var targetIndex = list.IndexOf(targetItem);

                //move source at the target's location
                list.Insert(targetIndex, DraggedItem);

                //select the dropped item
                dgQuoteCommodities.SelectedItem = DraggedItem;

                ReindexCollection((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource);
            }

            //reset
            ResetDragDrop();
        }


        /// <summary>
        /// Closes the popup and resets the
        /// grid to read-enabled mode.
        /// </summary>
        private void ResetDragDrop()
        {
            IsDragging = false;
            popup.IsOpen = false;
            dgQuoteCommodities.IsReadOnly = false;
        }


        /// <summary>
        /// Updates the popup's position in case of a drag/drop operation.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

            //display the popup if it hasn't been opened yet
            if (!popup.IsOpen)
            {
                //switch to read-only mode
                dgQuoteCommodities.IsReadOnly = true;

                //make sure the popup is visible
                popup.IsOpen = true;
            }


            Size popupSize = new Size(popup.ActualWidth, popup.ActualHeight);
            popup.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

            //make sure the row under the grid is being selected
            Point position = e.GetPosition(dgQuoteCommodities);
            var row = UIHelpers.TryFindFromPoint<DataGridRow>(dgQuoteCommodities, position);
            if (row != null) dgQuoteCommodities.SelectedItem = row.Item;
        }

        #endregion

        

    }
}

