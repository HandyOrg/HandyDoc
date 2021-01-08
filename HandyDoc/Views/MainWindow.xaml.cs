using HandyControl.Controls;
using HandyControl.Data;
using HandyDoc.ViewModels;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HandyDoc.Views
{
    public partial class MainWindow
    {
        internal static MainWindow Instance;
        private readonly List<NavigationViewItem> _controlPages = new List<NavigationViewItem>();

        public MainWindow()
        {
            InitializeComponent();
            cmbLang.SelectedIndex = GlobalDataHelper<AppConfig>.Config.Lang.Contains("English") ? 0 : 1;

            GetItemsForSearch();
        }

        #region Change Skin
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = true;
        }

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is SkinType tag)
            {
                PopupConfig.IsOpen = false;
                GlobalDataHelper<AppConfig>.Config.Skin = tag;
                GlobalDataHelper<AppConfig>.Save();
                ((App)Application.Current).UpdateSkin(tag);
            }
        }
        #endregion

        private void Control_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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
                        controlsSearchBox.ItemsSource = suggestions;
                    }
                }
                else
                {
                    controlsSearchBox.ItemsSource = new string[] { "No results found" };
                }
            }
        }
        private void Control_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
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

        public void GetItemsForSearch()
        {
            _controlPages?.Clear();
            foreach (var o in MainWindowViewModel.Instance.NavigationView)
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
    }
}
