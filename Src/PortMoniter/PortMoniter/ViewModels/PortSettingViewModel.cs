using System.Windows.Input;
using PortMoniter.Controls;
using PortMoniter.PartialViews;

namespace PortMoniter.ViewModels
{
    public class PortSettingViewModel : BaseViewModel
    {
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public IView View { get; }

        public PortSettingViewModel(IView view)
        {
            this.View = view;

            this.OkCommand = new RelayCommand(OkAction);
            this.CancelCommand = new RelayCommand(CancelAction);
        }

        public void OkAction()
        {
            this.View.CloseDialog(true); // close it with a successful result
        }

        public void CancelAction()
        {
            this.View.CloseDialog(false); // close it with a failed result
        }
    }
}
