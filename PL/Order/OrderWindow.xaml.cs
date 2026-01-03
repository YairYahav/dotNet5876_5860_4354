using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BlApi;
using BO;

namespace PL.Order
{
    public partial class OrderWindow : Window
    {
        private static readonly IBl s_bl = Factory.Get();

        private readonly int _managerId;
        private readonly int? _orderId; // null = Add mode

        public bool IsAddMode => _orderId is null;

        public string WindowTitle => IsAddMode ? "הוספת הזמנה" : "ניהול הזמנה בודדת";

        public BO.Order CurrentOrder
        {
            get { return (BO.Order)GetValue(CurrentOrderProperty); }
            set { SetValue(CurrentOrderProperty, value); }
        }

        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register(nameof(CurrentOrder), typeof(BO.Order), typeof(OrderWindow),
                new PropertyMetadata(null));

        // רשימת משלוחים - עכשיו תומך ברשימה מלאה
        public IEnumerable<DeliveryPerOrderInList> Deliveries
        {
            get { return (IEnumerable<DeliveryPerOrderInList>)GetValue(DeliveriesProperty); }
            set { SetValue(DeliveriesProperty, value); }
        }

        public static readonly DependencyProperty DeliveriesProperty =
            DependencyProperty.Register(nameof(Deliveries), typeof(IEnumerable<DeliveryPerOrderInList>), typeof(OrderWindow),
                new PropertyMetadata(null));

        public OrderWindow(int managerId, int? orderId = null)
        {
            _managerId = managerId;
            _orderId = orderId;

            InitializeComponent();

            Title = WindowTitle;

            if (IsAddMode)
            {
                CurrentOrder = CreateEmptyOrderForAdd();
                Deliveries = new List<DeliveryPerOrderInList>();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Order.AddObserver(OrderListObserver);

                if (!IsAddMode)
                    LoadOrder();
            }
            catch
            {
                MessageBox.Show("שגיאה בטעינת פרטי ההזמנה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                s_bl.Order.RemoveObserver(OrderListObserver);
            }
            catch
            {
                // לא מציגים שגיאות בסגירה
            }
        }

        private void OrderListObserver()
        {
            // אם החלון במצב Add אין מה לרענן
            if (IsAddMode) return;

            Dispatcher.Invoke(() =>
            {
                try { LoadOrder(); }
                catch { }
            });
        }

        private void LoadOrder()
        {
            if (_orderId is null)
                return;

            var o = s_bl.Order.GetOrder(_managerId, _orderId.Value);
            CurrentOrder = o;

            // תיקון: עכשיו משתמשים ב-DeliveriesForOrder (רשימה)
            Deliveries = o.DeliveriesForOrder ?? new List<DeliveryPerOrderInList>();
        }

        private void BtnPrimary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsAddMode)
                {
                    s_bl.Order.CreateOrder(_managerId, CurrentOrder);
                    MessageBox.Show("ההזמנה נוספה בהצלחה.", "הוספת הזמנה", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    s_bl.Order.UpdateOrder(_managerId, CurrentOrder);
                    MessageBox.Show("ההזמנה עודכנה בהצלחה.", "עדכון הזמנה", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    IsAddMode ? $"לא ניתן להוסיף את ההזמנה.\n{ex.Message}" : $"לא ניתן לעדכן את ההזמנה.\n{ex.Message}",
                    "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_orderId is null)
                    return;

                s_bl.Order.CancelOrder(_managerId, _orderId.Value);
                MessageBox.Show("ההזמנה בוטלה.", "ביטול הזמנה", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"לא ניתן לבטל את ההזמנה.\n{ex.Message}", "ביטול הזמנה",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private static BO.Order CreateEmptyOrderForAdd()
        {
            return new BO.Order
            {
                Id = 0,
                orderType = OrderType.Regular,
                DescriptionOfOrder = "",
                AddressOfOrder = "",
                Latitude = 0,
                Longitude = 0,
                AirDistance = 0,
                CustomerName = "",
                CustomerPhone = "",
                IsFrag = false,
                Volume = null,
                Weight = null,
                OrderPlacementTime = null,
                ExpectedCompletionTime = null,
                MaxDeliveryTime = DateTime.MinValue,
                OrderStatus = OrderStatus.Open,
                ScheduleStatus = ScheduleStatus.OnTime,
                RemainingTimeToDelivery = DateTime.MinValue,
                DeliveriesForOrder = null
            };
        }
    }
}
