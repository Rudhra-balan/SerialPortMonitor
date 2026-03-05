using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using PortMoniter.Pages;
using PortMoniter.PartialViews;
using VirtualPortBus;

namespace PortMoniter
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, IView
    {
        private MainWindow _portMoniter;
        private Shell _serialCom;

        public Dashboard()
        {
            try
            {
                InitializeComponent();
                _serialCom = new Shell();
                if (IsNotElevatedOrAdmin())
                {
                    sinffer.Visibility = Visibility.Hidden;
                    serialComm.IsChecked = true;
                    SerialCommClick(this, null);
                }
                else
                {
                    _portMoniter = new MainWindow();
                    sinffer.IsChecked = true;
                    SnifferClick(this, null);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        private bool IsNotElevatedOrAdmin()
        {
            return  !UacHelper.IsAdministrator();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
         
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SnifferClick(object sender, RoutedEventArgs e)
        {
            if (IsNotElevatedOrAdmin()) return;

            PagesNavigation.Navigate(_portMoniter);
        }

        private void SerialCommClick(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(_serialCom);
        }


        public void CloseDialog(bool result)
        {
           Close();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
          

            var theams = new List<Uri>()
            {
                new Uri("Assets/Styles/Themes/Dark/Theme.Colors.xaml", UriKind.Relative),
                new Uri("Assets/Styles/Themes/Dark/Styles.Shared.xaml", UriKind.Relative),
                new Uri("Assets/Styles/Themes/Dark/Dark.xaml", UriKind.Relative)
            };
           
            AppTheme.ChangeTheme(theams);
            
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            List<Uri> theams = new List<Uri>()
            {
                new Uri("Assets/Styles/Themes/Light/Theme.Colors.xaml", UriKind.Relative),
                new Uri("Assets/Styles/Themes/Light/Styles.Shared.xaml", UriKind.Relative),
                new Uri("Assets/Styles/Themes/Light/Light.xaml", UriKind.Relative)
            };
            AppTheme.ChangeTheme(theams);


        }
    }
}
