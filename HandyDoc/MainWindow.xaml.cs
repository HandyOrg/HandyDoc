using HandyControl.Themes;
using HandyControl.Tools;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HandyDoc
{
    public partial class MainWindow
    {
        private List<NavigationViewItem> _controlPages = new List<NavigationViewItem>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Change Theme
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button)
            {
                PopupConfig.IsOpen = false;
                if (button.Tag is ApplicationTheme tag)
                {
                    ((App)Application.Current).UpdateTheme(tag);
                }
            }
        }

        #endregion

        private void controlsSearchBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is string)
            {
                var item = _controlPages.FirstOrDefault(i => i.Content.ToString().Equals(args.QueryText, StringComparison.OrdinalIgnoreCase));
                navView.SelectedItem = item;
                navView.UpdateLayout();
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                var item = _controlPages.FirstOrDefault(i => i.Content.ToString().Equals(args.QueryText, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    navView.SelectedItem = item;
                    navView.UpdateLayout();
                }
            }
        }

        private void controlsSearchBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            var suggestions = new List<string>();

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var querySplit = sender.Text.Split(' ');
                var matchingItems = _controlPages.Where(
                    item =>
                    {
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            if (item.Content.ToString().IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                flag = false;
                            }

                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    suggestions.Add(item.Content.ToString());
                }
                if (suggestions.Count > 0)
                {
                    for (int i = 0; i < suggestions.Count; i++)
                    {
                        autoBox.ItemsSource = suggestions;
                    }
                }
                else
                {
                    autoBox.ItemsSource = new string[] { "No results found" };
                }
            }
        }
        public void GetItemsForSearch()
        {
            _controlPages?.Clear();
            foreach (var o in navView.MenuItems)
            {
                if (o is NavigationViewItem item)
                {
                    _controlPages.Add(item);
                    if (item.MenuItems != null)
                    {
                        foreach (var itemMenuItem in item.MenuItems)
                        {
                            if (itemMenuItem is NavigationViewItem item2)
                            {
                                _controlPages.Add(item2);
                            }
                        }
                    }
                }
            }
        }
        private async void navView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                mdText.Text = await ReadAndFixMarkdown(selectedItem.Tag.ToString());
                mdText.Tag = selectedItem.Tag.ToString();
                cmbLang.SelectedIndex = -1;
            }
        }
        private void LoadDocs(string lang)
        {
            navView.MenuItems.Add(new NavigationViewItemHeader() { Content = "Quick Start" });
            navView.MenuItems.Add(new NavigationViewItem() { Content = "Quick Start", Tag = @$"{lang}\handycontrol\quick_start", Icon = new SymbolIcon(Symbol.Home) });
            navView.MenuItems.Add(new NavigationViewItem() { Content = "Intellisense", Tag = @$"{lang}\handycontrol\intellisense", Icon = new SymbolIcon(Symbol.AllApps) });

            navView.MenuItems.Add(new NavigationViewItem() { Content = "Breaking Changes", Tag = @$"{lang}\handycontrol\breaking_changes", Icon = new SymbolIcon(Symbol.Character) });
            navView.MenuItems.Add(new NavigationViewItem() { Content = "Language", Tag = @$"{lang}\handycontrol\langs", Icon = new SymbolIcon(Symbol.Globe) });

            var expression = new NavigationViewItem() { Content = "Expression", Tag = @$"{lang}\handycontrol\expression", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("XamlGeometry") } };
            var media = new NavigationViewItem() { Content = "Media", Tag = @$"{lang}\handycontrol\media", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("ToolsGeometry") } };

            string pMediaPath = @$"{lang}\handycontrol\media";
            IEnumerable<string> mediaPath = Directory.EnumerateDirectories(pMediaPath);

            string pExpressionPath = @$"{lang}\handycontrol\expression";
            IEnumerable<string> expressionPath = expressionPath = Directory.EnumerateDirectories(pExpressionPath);


            foreach (var item in mediaPath)
            {
                var info = new DirectoryInfo(item);
                media.MenuItems.Add(new NavigationViewItem()
                { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }
            navView.MenuItems.Add(media);

            foreach (var item in expressionPath)
            {
                var info = new DirectoryInfo(item);
                expression.MenuItems.Add(new NavigationViewItem()
                { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }
            navView.MenuItems.Add(expression);

            navView.MenuItems.Add(new NavigationViewItem() { Content = "Theme", Tag = @$"{lang}\handycontrol\theme", Icon = new PathIcon() { Data = ResourceHelper.GetResource<Geometry>("PaletteGeometry") } });

            navView.MenuItems.Add(new NavigationViewItem() { Content = "Thanks", Tag = @$"{lang}\handycontrol\tnx", Icon = new SymbolIcon(Symbol.Like) });

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
            IEnumerable<string> persianToolkitPath = Directory.EnumerateDirectories(pToolPath);
            
            var dataPath = Directory.EnumerateDirectories(@$"{lang}\handycontrol\data");

            string interPath = @$"{lang}\handycontrol\interactivity";
            IEnumerable<string> interactivityPath = Directory.EnumerateDirectories(@interPath);
            
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

            foreach (var item in persianToolkitPath)
            {
                var info = new DirectoryInfo(item);
                persianToolkit.MenuItems.Add(new NavigationViewItem()
                { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }

            foreach (var item in dataPath)
            {
                var info = new DirectoryInfo(item);
                data.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }

            foreach (var item in interactivityPath)
            {
                var info = new DirectoryInfo(item);
                interactivity.MenuItems.Add(new NavigationViewItem()
                { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }

            foreach (var item in toolsPath)
            {
                var info = new DirectoryInfo(item);
                tools.MenuItems.Add(new NavigationViewItem() { Content = FirstCharToUpper(info.Name), Tag = info.FullName });
            }

            navView.MenuItems.Add(basicXaml);
            navView.MenuItems.Add(new NavigationViewItemHeader() { Content = "Controls and Styles" });
            navView.MenuItems.Add(nativeControls);
            navView.MenuItems.Add(extendControls);
            navView.MenuItems.Add(persianToolkit);
            navView.MenuItems.Add(tools);

            navView.MenuItems.Add(new NavigationViewItemHeader() { Content = "Interactivity" });

            navView.MenuItems.Add(interactivity);
            navView.MenuItems.Add(attach);
            navView.MenuItems.Add(data);

            try
            {
                
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (ArgumentOutOfRangeException) { }
        }
        public string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDocs(@"Docs\English");
            GetItemsForSearch();

            mdText.Text = await ReadAndFixMarkdown(@"Docs\English\handycontrol");
            mdText.Tag = @"Docs\English\handycontrol";
        }

        private async Task<string> ReadAndFixMarkdown(string path)
        {
            if (Directory.Exists(path))
            {
                var init = await File.ReadAllTextAsync(path + @"\index.md");
                var fixCode = init.Replace("{% code %}", "```").Replace("{% endcode %}", "```").
                    Replace("{% code lang:xml %}", "```").Replace("{% endcode %}", "```").
                    Replace("{% code lang:csharp %}", "```").Replace("{% endcode %}", "```").
                    Replace("{% code lang:cs %}", "```").Replace("{% endcode %}", "```")
                    .Replace("{% note warning %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note info %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note info no-icon %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note warning no-icon %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "");
                return fixCode;
            }
            return null;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(mdText.Text))
            {
                string location = string.Empty;
                if (cmbLang.SelectedIndex == 0)
                {
                    location = mdText.Tag.ToString().Replace(@"Docs\Chinese", @"Docs\English");
                }
                else
                {
                    location = mdText.Tag.ToString().Replace(@"Docs\English", @"Docs\Chinese");
                }

                mdText.Text = await ReadAndFixMarkdown(location);
            }
        }
    }
}
