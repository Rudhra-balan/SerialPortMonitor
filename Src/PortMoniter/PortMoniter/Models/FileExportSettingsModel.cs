using System;

namespace PortMoniter.Models
{
    [Serializable]
    public class FileExportSettingsModel : SingletonBase<FileExportSettingsModel>
    {
        public FileExportSettingsModel()
        {

        }

        public string[] getFileExtensions = { ".TXT", ".CSV" };
    }
}
