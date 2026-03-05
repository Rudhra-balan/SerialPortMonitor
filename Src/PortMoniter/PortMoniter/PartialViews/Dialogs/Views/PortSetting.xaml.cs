using System.Windows;
using PortMoniter.ViewModels;

namespace PortMoniter.PartialViews.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for PortSetting.xaml
    /// </summary>
    public partial class PortSetting : Window, IView
    {
        public PortSetting()
        {
            InitializeComponent();
            DataContext = new PortSettingViewModel(this);
        }
        public void CloseDialog(bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}
