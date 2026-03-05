using System.Threading.Tasks;
using PortMoniter.PartialViews.Dialogs.Models;

namespace PortMoniter.PartialViews.Dialogs.Interfaces
{
    public interface IProductKeyImportDialogServices
    {
        Task<ProductKeyImportResult> ShowFileExportDialogAsync();

        ProductKeyImportResult ShowFileExportDialog();

    }
}
