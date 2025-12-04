using System.Configuration;
using System.Data;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ChangeTheme(bool isDark)
        {
            var dictionary = new ResourceDictionary();
            string themeName = isDark ? "Dark" : "Light";
            dictionary.Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative);

            this.Resources.MergedDictionaries[0] = dictionary;
        }
    }

}
