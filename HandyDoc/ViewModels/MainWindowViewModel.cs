using HandyControl.Controls;
using HandyControl.Tools;
using ModernWpf.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using HandyDoc.Views;

namespace HandyDoc.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        internal static MainWindowViewModel Instance;
        private readonly IEventAggregator _ea;
        private readonly IRegionManager _region;

        private ObservableCollection<object> _navigationView = new ObservableCollection<object>();
        public ObservableCollection<object> NavigationView
        {
            get => _navigationView;
            set => SetProperty(ref _navigationView, value);
        }

        private DelegateCommand<SelectionChangedEventArgs> _languageSwitchCommand;
        public DelegateCommand<SelectionChangedEventArgs> LanguageSwitchCommand =>
            _languageSwitchCommand ??= new DelegateCommand<SelectionChangedEventArgs>(LanguageSwitch);

        private DelegateCommand<NavigationViewSelectionChangedEventArgs> _switchCommand;
        public DelegateCommand<NavigationViewSelectionChangedEventArgs> SwitchCommand =>
            _switchCommand ??= new DelegateCommand<NavigationViewSelectionChangedEventArgs>(OnSwitch);

        private string _currentLanguage;

        public MainWindowViewModel(IEventAggregator ea, IRegionManager regionManager)
        {
            Instance = this;
            _ea = ea;
            _region = regionManager;
            _currentLanguage = GlobalDataHelper<AppConfig>.Config.Lang;
            LoadDocs(@"Docs\" + _currentLanguage);
        }
        private void LanguageSwitch(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            if (e.AddedItems[0] is ComboBoxItem item)
            {
                GlobalDataHelper<AppConfig>.Config.Lang = item.Content.ToString();
                GlobalDataHelper<AppConfig>.Save();
                _currentLanguage = item.Content.ToString();
                LoadDocs(@"Docs\" + item.Content.ToString());
                MainWindow.Instance.GetItemsForSearch();
            }
        }
        private void LoadDocs(string lang)
        {
            try
            {
                NavigationView?.Clear();

                NavigationView?.Add(new NavigationViewItemHeader() { Content = "Quick Start" });
                NavigationView?.Add(new NavigationViewItem() { Content = "Quick Start", Tag = @$"{lang}\handycontrol\quick_start", Icon = new SymbolIcon(Symbol.Home) });
                if (_currentLanguage.Contains("English"))
                {
                    NavigationView?.Add(new NavigationViewItem() { Content = "Intellisense", Tag = @$"{lang}\handycontrol\intellisense", Icon = new SymbolIcon(Symbol.AllApps) });

                }
                NavigationView?.Add(new NavigationViewItem() { Content = "Breaking Changes", Tag = @$"{lang}\handycontrol\breaking_changes", Icon = new SymbolIcon(Symbol.Character) });
                NavigationView?.Add(new NavigationViewItem() { Content = "Language", Tag = @$"{lang}\handycontrol\langs", Icon = new SymbolIcon(Symbol.Globe) });
                if (_currentLanguage.Contains("English"))
                {
                    NavigationView?.Add(new NavigationViewItem() { Content = "Theme", Tag = @$"{lang}\handycontrol\theme", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("PaletteGeometry") } });
                }
                NavigationView?.Add(new NavigationViewItem() { Content = "Thanks", Tag = @$"{lang}\handycontrol\tnx", Icon = new SymbolIcon(Symbol.Like) });

                var basicXaml = new NavigationViewItem() { Content = "Basic Xaml", Tag = @$"{lang}\handycontrol\basic_xaml", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("XamlGeometry") } };

                var nativeControls = new NavigationViewItem() { Content = "Styles", Tag = @$"{lang}\handycontrol\native_controls", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("PaletteGeometry") } };
                var extendControls = new NavigationViewItem() { Content = "Custom Controls", Tag = @$"{lang}\handycontrol\extend_controls", Icon = new SymbolIcon(Symbol.AllApps) };
                var persianToolkit = new NavigationViewItem() { Content = "PersianToolkit", Tag = @$"{lang}\handycontrol\persianToolkit", Icon = new SymbolIcon(Symbol.Home) };


                var attach = new NavigationViewItem() { Content = "Attached Property", Tag = @$"{lang}\handycontrol\attach", Icon = new SymbolIcon(Symbol.Attach) };
                var data = new NavigationViewItem() { Content = "Data", Tag = @$"{lang}\handycontrol\data", Icon = new SymbolIcon(Symbol.Admin) };
                var interactivity = new NavigationViewItem() { Content = "Interactivity", Tag = @$"{lang}\handycontrol\interactivity", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("InterActivityGeometry") } };
                var tools = new NavigationViewItem() { Content = "Tools", Tag = @$"{lang}\handycontrol\tools", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("ToolsGeometry") } };

                var basicXamlPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\basic_xaml");
                var attachPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\attach");
                var nativeControlsPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\native_controls");
                var extendControlsPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\extend_controls");

                string pToolPath = @$"{lang}\handycontrol\persianToolkit";
                IEnumerable<string> persianToolkitPath = null;
                if (Directory.Exists(pToolPath))
                {
                    persianToolkitPath = Directory.EnumerateDirectories(pToolPath);
                }

                var dataPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\data");

                string interPath = @$"{lang}\handycontrol\interactivity";
                IEnumerable<string> interactivityPath = null;
                if (Directory.Exists(interPath))
                {
                    interactivityPath = Directory.EnumerateDirectories(@interPath);
                }

                var toolsPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\tools");

                foreach (var item in basicXamlPath)
                {
                    var info = new DirectoryInfo(item);
                    basicXaml.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }
                foreach (var item in attachPath)
                {
                    var info = new DirectoryInfo(item);
                    attach.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }
                foreach (var item in nativeControlsPath)
                {
                    var info = new DirectoryInfo(item);
                    nativeControls.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }
                foreach (var item in extendControlsPath)
                {
                    var info = new DirectoryInfo(item);
                    extendControls.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }

                if (persianToolkitPath != null)
                {
                    foreach (var item in persianToolkitPath)
                    {
                        var info = new DirectoryInfo(item);
                        persianToolkit.MenuItems.Add(new NavigationViewItem()
                        { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                    }
                }

                foreach (var item in dataPath)
                {
                    var info = new DirectoryInfo(item);
                    data.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }

                if (interactivityPath != null)
                {
                    foreach (var item in interactivityPath)
                    {
                        var info = new DirectoryInfo(item);
                        interactivity.MenuItems.Add(new NavigationViewItem()
                        { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                    }
                }

                foreach (var item in toolsPath)
                {
                    var info = new DirectoryInfo(item);
                    tools.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
                }

                NavigationView?.Add(basicXaml);
                NavigationView?.Add(new NavigationViewItemHeader() { Content = "Controls and Styles" });
                NavigationView?.Add(nativeControls);
                NavigationView?.Add(extendControls);
                if (persianToolkitPath != null)
                {
                    NavigationView?.Add(persianToolkit);
                }
                NavigationView?.Add(tools);

                NavigationView?.Add(new NavigationViewItemHeader() { Content = "Interactivity" });

                if (persianToolkitPath != null)
                {
                    NavigationView?.Add(interactivity);
                }
                NavigationView?.Add(attach);
                NavigationView?.Add(data);
            }
            catch (IndexOutOfRangeException)
            {
            }
        }
        public string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        private void OnSwitch(NavigationViewSelectionChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationViewItem item)
            {
                if (item.Tag is not null)
                {
                    if (item.Tag is not "Settings")
                    {
                        _ea.GetEvent<PubSubEvent<string>>().Publish(item.Tag.ToString());
                    }
                    else
                    {
                        _region.RequestNavigate("ContentRegion", "Settings");
                    }
                }
            }
        }
    }
}
