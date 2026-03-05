using System.Windows;
using PortMoniter.ViewModels;

namespace PortMoniter.Pages
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell 
    {
        public Shell()
        {
            InitializeComponent();

            SerialCommViewModel viewModel = new SerialCommViewModel();
            this.DataContext = viewModel;
           
        }
    }
}
