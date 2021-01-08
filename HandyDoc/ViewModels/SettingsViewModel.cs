using HandyControl.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Reflection;

namespace HandyDoc.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private DelegateCommand _checkUpdateCommand;
        public DelegateCommand CheckUpdateCommand =>
            _checkUpdateCommand ??= new DelegateCommand(OnCheckUpdate);

        private string _version;

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
        public SettingsViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }

        private void OnCheckUpdate()
        {
            try
            {
                var ver =
                    UpdateHelper.CheckForUpdateGithubRelease("HandyOrg", "HandyDoc");

                if (ver.IsExistNewVersion)
                {
                    Growl.AskGlobal("we found a new Version, do you want to download?", b =>
                    {
                        if (!b)
                        {
                            return true;
                        }

                        Process.Start(ver.Asset[0].browser_download_url);
                        return true;
                    });
                }
                else
                {
                    Growl.InfoGlobal("you are using Latest Version.");
                }
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ex.Message);
            }
        }
    }
}
