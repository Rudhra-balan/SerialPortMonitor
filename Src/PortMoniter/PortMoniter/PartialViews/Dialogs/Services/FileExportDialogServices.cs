using System.Threading.Tasks;
using System.Windows;
using PortMoniter.PartialViews.Dialogs.Interfaces;
using PortMoniter.PartialViews.Dialogs.Models;
using PortMoniter.PartialViews.Dialogs.Views;

namespace PortMoniter.PartialViews.Dialogs.Services
{
    public class FileExportDialogServices : IFileExportDialogServices
    {
        public async Task<FileExportDialogResult> ShowFileExportDialogAsync()
        {
            return await Application.Current.Dispatcher.InvokeAsync(ShowFileExportDialog);
        }

        public FileExportDialogResult ShowFileExportDialog()
        {
            FileExport portSetting = new FileExport();
          //  portSetting.ViewModel.OutputText = ExportData;
            bool result = portSetting.ShowDialog() == true;
            if (!result)
            {
                // DialogResult failed, so return an empty result with a failure state
                return new FileExportDialogResult { IsSuccess = false };
            }
            return new FileExportDialogResult()
            {
                IsSuccess = true
            };
        }

        public string ExportData { get; set; }
    }
}
