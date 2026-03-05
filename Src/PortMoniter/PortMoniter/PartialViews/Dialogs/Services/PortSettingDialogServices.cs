using System.Threading.Tasks;
using System.Windows;
using PortMoniter.PartialViews.Dialogs.Interfaces;
using PortMoniter.PartialViews.Dialogs.Models;
using PortMoniter.PartialViews.Dialogs.Views;

namespace PortMoniter.PartialViews.Dialogs.Services
{
    public class PortSettingDialogServices : ISettingsDialogService
    {
        public async Task<PortSettingDialogResult> ShowSettingDialogAsync()
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return ShowSettingDialog();
            });
        }

        public PortSettingDialogResult ShowSettingDialog()
        {
            PortSetting portSetting = new PortSetting();
            bool result = portSetting.ShowDialog() == true;
            if (!result)
            {
                // DialogResult failed, so return an empty result with a failure state
                return new PortSettingDialogResult() { IsSuccess = false };
            }

            return new PortSettingDialogResult()
            {
                IsSuccess = true
            };
        }
    }
}
