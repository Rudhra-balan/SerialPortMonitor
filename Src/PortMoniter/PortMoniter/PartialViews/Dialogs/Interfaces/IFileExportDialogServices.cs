using System.Threading.Tasks;
using PortMoniter.PartialViews.Dialogs.Models;

namespace PortMoniter.PartialViews.Dialogs.Interfaces
{
    public interface IFileExportDialogServices
    {
        Task<FileExportDialogResult> ShowFileExportDialogAsync();

        FileExportDialogResult ShowFileExportDialog();

        string ExportData { get; set; }
      
    }
}
