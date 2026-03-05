using PortMoniter.ViewModels;

namespace PortMoniter.Pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
      
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

        }

       


    }
}
