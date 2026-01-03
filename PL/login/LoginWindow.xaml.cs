using PL.Courier;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PL.login;

public partial class LoginWindow : Window, INotifyPropertyChanged
{
    private readonly BlApi.IBl _bl = BlApi.Factory.Get();

    private string _userIdText = "";
    public string UserIdText
    {
        get => _userIdText;
        set { _userIdText = value; OnPropertyChanged(); }
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        private set { _password = value; OnPropertyChanged(); }
    }

    public LoginWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        Password = ((PasswordBox)sender).Password;
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!TryParseIsraeliId(UserIdText, out int id))
                throw new ArgumentException("תעודת זהות לא תקינה. הזן 9 ספרות עם ספרת ביקורת תקינה.");

            if (string.IsNullOrWhiteSpace(Password))
                throw new ArgumentException("יש להזין סיסמה.");

            // קריאה אחת ל BL
            var result = _bl.Courier.Login(id, Password);

            if (result == BO.UserRole.Courier)
            {
                var courier = new CourierWindow(id);
                courier.Show();
            }
            else
            {
                var admin = new MainWindow();
                admin.Show();
            }
            
            Close();
        }
        catch
        {
            MessageBox.Show(
                "פרטי התחברות שגויים",
                "כניסה נכשלה",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
        finally
        {
            // לא להשאיר סיסמה בזיכרון של ה UI מעבר לנדרש
            Password = "";
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        UserIdText = "";
        Password = "";
    }

    private static bool TryParseIsraeliId(string? text, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        var s = text.Trim();
        if (s.Length > 9) return false;
        s = s.PadLeft(9, '0');
        if (!s.All(char.IsDigit)) return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = s[i] - '0';
            int mult = (i % 2 == 0) ? digit : digit * 2;
            if (mult > 9) mult -= 9;
            sum += mult;
        }
        if (sum % 10 != 0) return false;

        id = int.Parse(s);
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
