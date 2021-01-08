using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using ModernWpf;
using System.Windows;

namespace HandyDoc
{
    public partial class App : Application
    {
        public App()
        {
            GlobalDataHelper<AppConfig>.Init();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var boot = new Bootstrapper();
            boot.Run();
            UpdateSkin(GlobalDataHelper<AppConfig>.Config.Skin);
        }

        internal void UpdateSkin(SkinType skin)
        {
            SharedResourceDictionary.SharedDictionaries.Clear();
            ResourceHelper.GetTheme("HandyTheme", Resources).Skin = skin;

            ThemeManager.Current.ApplicationTheme = skin == SkinType.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
            Current.MainWindow?.OnApplyTemplate();
        }
    }
}
