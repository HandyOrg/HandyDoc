using HandyControl.Themes;
using System.Windows;

namespace HandyDoc
{
    public partial class App : Application
    {
        internal void UpdateTheme(ApplicationTheme theme)
        {
            ThemeManager.Current.ApplicationTheme = theme;
            ModernWpf.ThemeManager.Current.ApplicationTheme = theme == ApplicationTheme.Light ? ModernWpf.ApplicationTheme.Light : ModernWpf.ApplicationTheme.Dark;
        }
    }
}
