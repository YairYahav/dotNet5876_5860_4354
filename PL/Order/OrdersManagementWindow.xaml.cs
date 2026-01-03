using BlApi;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PL.Order;

public partial class OrdersManagementWindow : Window
{
    static readonly IBl s_bl = Factory.Get();
    private readonly int _managerId;


    public OrdersManagementWindow(int managerId)
    {
        _managerId = managerId;

        CancelOrderCommand = new RelayCommand(
            execute: p => CancelOrder(p),
            canExecute: p => p is int);

        InitializeComponent();

        // ערכי ברירת מחדל
        StatusFilter = null;                  // אין סינון
        OrderBy = OrderListOrderBy.ById;      // מיון מינימלי
        BuildOrderStatusValues();
    }

    // הרשימה לתצוגה
    public IEnumerable<OrderInList> OrderList
    {
        get { return (IEnumerable<OrderInList>)GetValue(OrderListProperty); }
        set { SetValue(OrderListProperty, value); }
    }
    public static readonly DependencyProperty OrderListProperty =
    DependencyProperty.Register(nameof(OrderList), typeof(IEnumerable<OrderInList>), typeof(OrdersManagementWindow),
        new PropertyMetadata(null));

    public OrderInList? SelectedOrder
    {
        get { return (OrderInList)GetValue(SelectedOrderProperty); }
        set { SetValue(SelectedOrderProperty, value); }
    }

    public static readonly DependencyProperty SelectedOrderProperty =
    DependencyProperty.Register(nameof(SelectedOrder), typeof(OrderInList), typeof(OrdersManagementWindow),
        new PropertyMetadata(null));

    // פילטר סטטוס: Nullable (null = ללא סינון)
    public OrderStatus? StatusFilter
    {
        get { return (OrderStatus?)GetValue(StatusFilterProperty); }
        set { SetValue(StatusFilterProperty, value); }
    }

    public static readonly DependencyProperty StatusFilterProperty =
        DependencyProperty.Register(nameof(StatusFilter), typeof(OrderStatus?), typeof(OrdersManagementWindow),
            new PropertyMetadata(null, OnFilterOrSortChanged));

    // מיון
    public OrderListOrderBy OrderBy
    {
        get { return (OrderListOrderBy)GetValue(OrderByProperty); }
        set { SetValue(OrderByProperty, value); }
    }

    public static readonly DependencyProperty OrderByProperty =
           DependencyProperty.Register(nameof(OrderBy), typeof(OrderListOrderBy), typeof(OrdersManagementWindow),
               new PropertyMetadata(OrderListOrderBy.ById, OnFilterOrSortChanged));
    private static void OnFilterOrSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OrdersManagementWindow w)
            w.RefreshList();
    }

    // מקור ל-ComboBox של הסטטוסים (כולל null בראש הרשימה)
    public List<OrderStatus?> OrderStatusValues
    {
        get { return (List<OrderStatus?>)GetValue(OrderStatusValuesProperty); }
        set { SetValue(OrderStatusValuesProperty, value); }
    }

    public static readonly DependencyProperty OrderStatusValuesProperty =
        DependencyProperty.Register(nameof(OrderStatusValues), typeof(List<OrderStatus?>), typeof(OrdersManagementWindow),
            new PropertyMetadata(null));

    private void BuildOrderStatusValues()
    {
        var vals = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus?>().ToList();
        vals.Insert(0, null); // null = ללא סינון
        OrderStatusValues = vals;
    }

    // Command לביטול
    public ICommand CancelOrderCommand { get; }

    private void RefreshList()
    {
        try
        {
            IEnumerable<OrderInList> temp;

            // אם יש פילטר סטטוס, נשתמש בפילטר המובנה של BL
            if (StatusFilter is not null)
            {
                temp = s_bl.Order.GetOrders(
                    _managerId,
                    OrderListFilterBy.ByStatus,
                    StatusFilter.Value,
                    OrderBy);
            }
            else
            {
                temp = s_bl.Order.GetOrders(
                    _managerId,
                    null,
                    null,
                    OrderBy);
            }

            OrderList = temp.ToList();
        }
        catch
        {
            MessageBox.Show("לא ניתן לטעון את רשימת ההזמנות.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OrdersListObserver()
    {
        Dispatcher.Invoke(RefreshList);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Order.AddObserver(OrdersListObserver);
            RefreshList();
        }
        catch
        {
            MessageBox.Show("שגיאה בטעינת מסך ניהול הזמנות.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        try { s_bl.Order.RemoveObserver(OrdersListObserver); }
        catch { }
    }

    private void OrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedOrder is null)
            return;

        // לפתוח מסך "ניהול הזמנה בודדת" עבור SelectedOrder.OrderId
        // חשוב: Show() כדי לא לחסום את מסך הרשימה
        new OrderWindow(_managerId, SelectedOrder.OrderId).Show();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        // פתיחת מסך הוספה (Show, לא לחסום)
        new OrderWindow(_managerId).Show();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CancelOrder(object? parameter)
    {
        try
        {
            if (parameter is not int orderId)
                return;

            s_bl.Order.CancelOrder(_managerId, orderId);

            // observer ירענן גם כן, אבל אפשר להשאיר לרענון מהיר
            RefreshList();
        }
        catch
        {
            MessageBox.Show("לא ניתן לבטל את ההזמנה. ייתכן שהיא כבר סגורה.", "ביטול הזמנה", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}

// RelayCommand בסיסי
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}



