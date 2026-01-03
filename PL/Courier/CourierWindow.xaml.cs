using System;
using System.Windows;
using BlApi;
using BO;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierWindow.xaml
    /// </summary>
    public partial class CourierWindow : Window
    {
        private static readonly IBl s_bl = Factory.Get();

        // Dependency Property for the bound Courier object
        public BO.Courier CurrentCourier
        {
            get { return (BO.Courier)GetValue(CurrentCourierProperty); }
            set { SetValue(CurrentCourierProperty, value); }
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierWindow), new PropertyMetadata(null));

        // Dependency Property for the Button Text ("Add" or "Update")
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CourierWindow), new PropertyMetadata("Add"));

        // Constructor accepting ID (0 for Add, other for Update)
        public CourierWindow(int id = 0)
        {
            InitializeComponent();

            try
            {
                if (id != 0)
                {
                    // Update Mode: Load existing courier
                    CurrentCourier = s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, id);
                    ButtonText = "Update";

                    // Register observer for this specific courier
                    s_bl.Courier.AddObserver(id, CourierObserver);
                }
                else
                {
                    // Add Mode: Create new empty courier
                    CurrentCourier = new BO.Courier
                    {
                        EmploymentStartDate = s_bl.Admin.GetClock(),
                        IsActive = true
                    };
                    ButtonText = "Add";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Courier", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    // Create new courier
                    s_bl.Courier.CreateCourier(s_bl.Admin.GetConfig().ManagerId, CurrentCourier);
                    MessageBox.Show("Courier added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Update existing courier
                    s_bl.Courier.UpdateCourier(s_bl.Admin.GetConfig().ManagerId, CurrentCourier);
                    MessageBox.Show("Courier updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Observer method to refresh data if updated externally
        private void CourierObserver()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    // Reload courier data
                    if (ButtonText == "Update" && CurrentCourier != null)
                    {
                        CurrentCourier = s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, CurrentCourier.Id);
                    }
                }
                catch (Exception) { /* Handle or ignore if window is closing */ }
            });
        }

        // Remove observer when closing
        protected override void OnClosed(EventArgs e)
        {
            if (ButtonText == "Update" && CurrentCourier != null)
            {
                try
                {
                    s_bl.Courier.RemoveObserver(CurrentCourier.Id, CourierObserver);
                }
                catch { }
            }
            base.OnClosed(e);
        }
    }
}