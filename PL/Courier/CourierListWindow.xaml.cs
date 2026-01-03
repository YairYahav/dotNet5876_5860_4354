using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        // Dependency Property for the list
        public IEnumerable<CourierInList> CourierList
        {
            get { return (IEnumerable<CourierInList>)GetValue(CourierListProperty); }
            set { SetValue(CourierListProperty, value); }
        }

        public static readonly DependencyProperty CourierListProperty =
            DependencyProperty.Register("CourierList", typeof(IEnumerable<CourierInList>), typeof(CourierListWindow), new PropertyMetadata(null));

        public DeliveryType DeliveryTypeFilter 
        { 
            get { return (DeliveryType)GetValue(DeliveryTypeFilterProperty); }
            set { SetValue(DeliveryTypeFilterProperty, value); }
        }

        public static readonly DependencyProperty DeliveryTypeFilterProperty =
            DependencyProperty.Register("DeliveryTypeFilter", typeof(DeliveryType), typeof(CourierListWindow), 
                new PropertyMetadata(DeliveryType.None, OnFilterChanged));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CourierListWindow window)
                window.RefreshList();
        }

        public CourierInList? SelectedCourier 
        { 
            get { return (CourierInList)GetValue(SelectedCourierProperty); }
            set { SetValue(SelectedCourierProperty, value); }
        }

        public static readonly DependencyProperty SelectedCourierProperty =
            DependencyProperty.Register("SelectedCourier", typeof(CourierInList), typeof(CourierListWindow), new PropertyMetadata(null));

        public CourierListWindow()
        {
            InitializeComponent();
        }

        private void RefreshList()
        {
            try
            {
                int managerId = s_bl.Admin.GetConfig().ManagerId;
                var temp = s_bl.Courier.GetCouriers(managerId);

                if (DeliveryTypeFilter != DeliveryType.None)
                {
                    temp = temp.Where(c => c.DeliveryType == DeliveryTypeFilter);
                }

                CourierList = temp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Couriers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CourierListObserver()
        {
            Dispatcher.Invoke(() => RefreshList());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Courier.AddObserver(CourierListObserver);
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Courier List", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                s_bl.Courier.RemoveObserver(CourierListObserver);
            }
            catch (Exception)
            {
                // Suppress errors during closing to avoid cascading issues
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow().Show();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCourier != null)
            {
                new CourierWindow(SelectedCourier.Id).Show();
            }
        }

        // Chapter 11: Handle Delete Button Click
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Get the item associated with the clicked button
            if (sender is Button btn && btn.DataContext is CourierInList courierToDelete)
            {
                // 1. Confirm deletion
                var result = MessageBox.Show(
                    $"Are you sure you want to delete courier: {courierToDelete.FullName}?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 2. Call BL to delete
                        int managerId = s_bl.Admin.GetConfig().ManagerId;
                        s_bl.Courier.DeleteCourier(managerId, courierToDelete.Id);

                        // 3. The list will update automatically thanks to the Observer (CourierListObserver)
                    }
                    catch (Exception ex)
                    {
                        // 4. Handle failure
                        MessageBox.Show(ex.Message, "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}