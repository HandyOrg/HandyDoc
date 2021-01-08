using HandyControl.Controls;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.IO;
using System.Threading.Tasks;

namespace HandyDoc.ViewModels
{
    public class MarkdownViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator _ea;
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public MarkdownViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<PubSubEvent<string>>().Subscribe(MessageReceived);
        }


        private async void MessageReceived(string message)
        {
            Message = await ReadAndFixMarkdown(message);
        }

        private async Task<string> ReadAndFixMarkdown(string path)
        {
            if (Directory.Exists(path))
            {
                var init = await File.ReadAllTextAsync(path + @"\index.md");
                var fixCode = init.Replace("{% code %}", "```").Replace("{% endcode %}", "```").
                    Replace("{% code lang:xml %}", "```").Replace("{% endcode %}", "```").
                    Replace("{% code lang:csharp %}", "```").Replace("{% endcode %}", "```")
                    .Replace("{% note warning %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note info %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note info no-icon %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "")
                    .Replace("{% note warning no-icon %}", "> **_NOTE:_**  ").Replace("{% endnote %}", "");
                return fixCode;
            }
            return null;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Message = await ReadAndFixMarkdown(@$"Docs\{GlobalDataHelper<AppConfig>.Config.Lang}\handycontrol");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
