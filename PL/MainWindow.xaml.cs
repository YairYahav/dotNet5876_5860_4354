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
using BlApi;
using BO;
using PL.Courier;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Access to the BL layer (Stage 6b)
    static readonly IBl s_bl = Factory.Get();

    public MainWindow()
    {
        InitializeComponent();
    }

    #region Dependency Properties (Stage 6d & 6f)

    // Dependency Property for System Clock
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    // Dependency Property for Configuration Object
    public Config Configuration
    {
        get { return (Config)GetValue(ConfigurationProperty); }
        set { SetValue(ConfigurationProperty, value); }
    }

    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.Register("Configuration", typeof(Config), typeof(MainWindow));

    #endregion

    #region Window Events (Stage 6h & 6i)

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Initial load of data
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();

            // Register observers
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            // Unregister observers to prevent memory leaks
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    #endregion

    #region Observers (Stage 6g)

    // Method called when the BL clock updates
    private void ClockObserver()
    {
        // Must be run on UI thread if called from a different thread (important for Stage 7)
        Dispatcher.Invoke(() =>
        {
            try
            {
                CurrentTime = s_bl.Admin.GetClock();
            }
            catch (Exception ex) { ShowError(ex); }
        });
    }

    // Method called when BL configuration updates
    private void ConfigObserver()
    {
        Dispatcher.Invoke(() =>
        {
            try
            {
                Configuration = s_bl.Admin.GetConfig();
            }
            catch (Exception ex) { ShowError(ex); }
        });
    }

    #endregion

    #region Clock Buttons (Stage 6e)

    private void BtnAddMinute_Click(object sender, RoutedEventArgs e) => ForwardClock(TimeUnit.MINUTE);
    private void BtnAddHour_Click(object sender, RoutedEventArgs e) => ForwardClock(TimeUnit.HOUR);
    private void BtnAddDay_Click(object sender, RoutedEventArgs e) => ForwardClock(TimeUnit.DAY);
    private void BtnAddYear_Click(object sender, RoutedEventArgs e) => ForwardClock(TimeUnit.YEAR);

    private void ForwardClock(TimeUnit unit)
    {
        try
        {
            s_bl.Admin.ForwardClock(unit);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    #endregion

    #region Configuration Actions (Stage 6f)

    private void BtnUpdateConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Configuration is already updated via TwoWay binding, we just send it to BL
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Configuration updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    #endregion

    #region Navigation & Database Actions (Stage 6j & 6k)

    private void BtnCouriers_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            new CourierListWindow().Show();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnInitDB_Click(object sender, RoutedEventArgs e)
    {
        HandleDbAction("Initialize", s_bl.Admin.InitializeDB);
    }

    private void BtnResetDB_Click(object sender, RoutedEventArgs e)
    {
        HandleDbAction("Reset", s_bl.Admin.ResetDB);
    }

    private void HandleDbAction(string actionName, Action dbAction)
    {
        var result = MessageBox.Show($"Are you sure you want to {actionName} the Database?", "Confirm Action",
                                     MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            Mouse.OverrideCursor = Cursors.Wait; // Show hourglass
            try
            {
                CloseAllOtherWindows();
                dbAction();
                MessageBox.Show($"Database {actionName} completed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh local data after DB change
                CurrentTime = s_bl.Admin.GetClock();
                Configuration = s_bl.Admin.GetConfig();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                Mouse.OverrideCursor = null; // Restore cursor
            }
        }
    }

    // Helper to close all windows except the main one
    private void CloseAllOtherWindows()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window != this)
            {
                window.Close();
            }
        }
    }

    #endregion

    #region Helpers

    private void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    #endregion

    // Theme Toggle Handler
    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Primitives.ToggleButton toggleButton)
        {
            // Access the App class to change the theme
            ((App)Application.Current).ChangeTheme(toggleButton.IsChecked == true);
        }
    }
}