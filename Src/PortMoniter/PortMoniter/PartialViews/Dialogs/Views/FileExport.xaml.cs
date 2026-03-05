using System.Windows;
using PortMoniter.ViewModels;

namespace PortMoniter.PartialViews.Dialogs.Views
{  /// <summary>
    /// Interaction logic for FileExport.xaml
    /// </summary>
    public partial class FileExport : Window, IView
    {
        public FileExportViewModel ViewModel;
        public FileExport()
        {
            InitializeComponent();
           // ViewModel = new FileExportViewModel(this);
            DataContext = ViewModel;
        }

        public void CloseDialog(bool result)
        {
            DialogResult = result;
            Close();
        }
    }

}
