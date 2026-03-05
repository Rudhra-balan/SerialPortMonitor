using System.Threading.Tasks;
using PortMoniter.PartialViews.Dialogs.Models;

namespace PortMoniter.PartialViews.Dialogs.Interfaces
{
    public interface ISettingsDialogService
    {
        // An async version that will just delegate the non-async version
        Task<PortSettingDialogResult> ShowSettingDialogAsync();

        PortSettingDialogResult ShowSettingDialog();
    }
}
