using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using PortMoniter.Controls;
using PortMoniter.Models;

namespace PortMoniter.ViewModels
{
    public class SerialCommViewModel : BaseViewModel
    {
        #region Private Fields
        private static string AppTitle = "Serial Communication";
        private SerialPort _SerialPort;
        private DispatcherTimer timer = null;
        private string _FileName = "output_data";
        private bool _IsAutoscrollChecked = true;
        private ICommand _Open;
        private ICommand _Close;
        private ICommand _Send;
        private ICommand _Clear;
        private ICommand _OpenLink;
        private ICommand _RefreshPorts;
        private ICommand _ChangeFileLocation;
        private ICommand _ExportTXTFile;
        #endregion

        #region Public Properties

        
        private bool _isHex = true;
        public bool IsHex
        {
            get => _isHex;
            set
            {
                _isHex = value;
                OnPropertyChanged("IsHex");
            }
        }

        public string InputText { get; set; }
        public string OutputText { get;  set; }
        public string WindowTitle { get; set; }
        public List<SerialPortSettingsModel> CommPorts { get;  set; }
        public SerialPortSettingsModel SelectedCommPort { get;  set; }
        public List<SerialPortSettingsModel> BaudRates { get;  set; }
        public int SelectedBaudRate { get;  set; }
        public List<SerialPortSettingsModel> Parities { get;  set; }
        public Parity SelectedParity { get;  set; }
        public List<SerialPortSettingsModel> FlowControlList { get;  set; }
        public List<SerialPortSettingsModel> StopBitsList { get;  set; }
        public StopBits SelectedStopBits { get;  set; }
        public Handshake SelectedFlowControl { get;  set; }
        public int[] DataBits { get; set; }
        public int SelectedDataBits { get;  set; }
        public List<SerialPortSettingsModel> LineEndings { get;  set; }
        public string SelectedLineEnding { get;  set; }
        public bool IsDTR { get;  set; }
        public bool IsRTS { get;  set; }
        public bool EnableDisableSettings { get;  set; }
        public string FileLocation { get;  set; }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public string[] FileExtensions { get; set; }
        public string SelectedFileExtension { get;  set; }
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnPropertyChanged("ExportFile");
            }
        }
        public string ExportStatus { get; set; }
        public bool ExportStatusSuccess { get; set; }

        private Visibility _stoptButtonVisibility;
        public Visibility StopButtonVisible
        {
            get => _stoptButtonVisibility;
            set
            {
                _stoptButtonVisibility = value;
                OnPropertyChanged("StopButtonVisible");
            }
        }

        private Visibility _startButtonVisibility;
        public Visibility StartButtonVisible
        {
            get => _startButtonVisibility;
            set
            {
                _startButtonVisibility = value;
                OnPropertyChanged("StartButtonVisible");
            }
        }

        // Disable interaction from UI
        // TODO: Trigger TextBoxAutomaticScrollingExtension.cs or Scroll to
        //       end the TextBox when CheckBox is checked for second time.
        public string ScrollConfirm
        {
            get
            {
                // Debug only
                //return "Autoscroll (" + ScrollOnTextChanged.ToString() + ")";
                return "Autoscroll";
            }
        }
        public bool ScrollOnTextChanged { get; set; }
        public bool IsAutoscrollChecked
        {
            get { return _IsAutoscrollChecked; }
            set
            {
                _IsAutoscrollChecked = value;
                if (_IsAutoscrollChecked == true)
                {
                    ScrollOnTextChanged = true;
                }
                else
                {
                    ScrollOnTextChanged = false;
                }

                OnPropertyChanged("IsAutoscrollChecked");
                OnPropertyChanged("ScrollOnTextChanged");
                OnPropertyChanged("ScrollConfirm");
            }
        }
        #endregion

        #region Public ICommands
        public ICommand Open
        {
            get
            {
                _Open = new ConditionalRelayCommand(
                    param => StartListening(),
                    param =>
                    {
                        var canStart = StartListeningCanExecute();
                        if (!canStart) return false;
                        StartButtonVisible = Visibility.Visible;
                        StopButtonVisible = Visibility.Hidden;

                        return true;
                    });
                return _Open;
            }
        }
        
        public ICommand Close
        {
            get
            {
                _Close = new ConditionalRelayCommand(
                    param => StopListening(),
                    param =>
                    {
                        var canClose = _SerialPort != null && _SerialPort.IsOpen;
                        if (!canClose) return false;
                        StartButtonVisible = Visibility.Hidden;
                        StopButtonVisible = Visibility.Visible;

                        return true;
                    });
                return _Close;
            }
        }
        
        public ICommand Send
        {
            get
            {
                _Send = new ConditionalRelayCommand(
                    param => WriteData(),
                    param => _SerialPort != null && _SerialPort.IsOpen);
                return _Send;
            }
        }

        public ICommand Clear
        {
            get
            {
                _Clear = new ConditionalRelayCommand(
                    param => ClearOutput(), param=> !string.IsNullOrEmpty(OutputText));
                return _Clear;
            }
        }

        
        public ICommand RefreshPorts
        {
            get
            {
                _RefreshPorts = new ConditionalRelayCommand(
                    param => RefreshPortsMethod(),
                    param => (_SerialPort == null || !_SerialPort.IsOpen));
                return _RefreshPorts;
            }
        }

        public ICommand ChangeFileLocation
        {
            get
            {
                _ChangeFileLocation = new ConditionalRelayCommand(
                    param => ChangeFileLocationMethod());
                return _ChangeFileLocation;
            }
        }

        public ICommand ExportFile
        {
            get
            {
                _ExportTXTFile = new ConditionalRelayCommand(
                    param => ExportFileMethod(),
                    param => ExportFileCanExecute());
                return _ExportTXTFile;
            }
        }
        #endregion

        #region Constructor
        public SerialCommViewModel()
        {
            StartButtonVisible = Visibility.Visible;
            StopButtonVisible = Visibility.Hidden;

            _SerialPort = new SerialPort();
            
            // Get lists of settings objects
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.getCommPorts();
                BaudRates = SerialPortSettingsModel.Instance.getBaudRates();
                Parities = SerialPortSettingsModel.Instance.getParities();
                DataBits = SerialPortSettingsModel.Instance.getDataBits;
                StopBitsList = SerialPortSettingsModel.Instance.getStopBits();
                FlowControlList = SerialPortSettingsModel.Instance.getFlowControl();
                LineEndings = SerialPortSettingsModel.Instance.getLineEndings();
                FileExtensions = FileExportSettingsModel.Instance.getFileExtensions;
               
            }
            catch (Exception)
            {
                //
            }

            // Set default values
            if (CommPorts != null && CommPorts.Any())
            {
                SelectedCommPort = CommPorts[0];
            }

            SelectedBaudRate = 9600;
            SelectedParity = Parity.None;
            SelectedDataBits = 8;
            SelectedStopBits = StopBits.One;
            SelectedFlowControl = Handshake.None;
            SelectedLineEnding = "";
            IsDTR = true;
            IsRTS = true;
            FileLocation = AssemblyDirectory;
            SelectedFileExtension = FileExtensions[0];
            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            EnableDisableSettings = true;
            ScrollOnTextChanged = true;
           
        }
        #endregion
        private static readonly StringBuilder FormatedString = new StringBuilder();
        #region Events
        /// <summary>
        /// Receive data event from serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    //string inData = _SerialPort.ReadLine();
                    //OutputText += inData.ToString() + SelectedLineEnding;
                    //OnPropertyChanged("OutputText");

                    byte[] data = new byte[_SerialPort.BytesToRead];
                    _SerialPort.Read(data, 0, data.Length);
                    string dataIn = ToHexString(data);
                    FormatedString.AppendLine(dataIn);
                    OutputText = FormatedString.ToString();
                    OnPropertyChanged("OutputText");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                   
                }
            }
        }

        string ToHexString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in bytes)
            {
                stringBuilder.Append(item.ToString("X"));
                stringBuilder.Append(" ");
            }

            return stringBuilder.ToString();
        }
        private void TimerTick(object send, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
                ExportStatus = "";
                OnPropertyChanged("ExportStatus");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Close port if port is open when user closes MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (_SerialPort != null && _SerialPort.IsOpen)
                {
                    _SerialPort.DataReceived -= DataReceivedEvent;
                    _SerialPort.Dispose();
                    _SerialPort.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Private Methods

        private void ClearOutput()
        {
            OutputText = string.Empty;
            OnPropertyChanged("OutputText");
        }
        
        /// <summary>
        /// Send data to serial port.
        /// </summary>
        private void WriteData()
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    if (IsHex)
                    {
                        var input= InputText.Replace(" ", "");
                        if(string.IsNullOrEmpty(input)) return;
                        var inputByte = Enumerable.Range(0, input.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(input.Substring(x, 2), 16))
                            .ToArray();

                        _SerialPort.Write(inputByte,0,inputByte.Length);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(InputText)) return;
                        _SerialPort.Write(InputText);
                    }
                   
                    InputText = String.Empty;
                    OnPropertyChanged("InputText");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Initiate serial port communication.
        /// </summary>
        private void StartListening()
        {
            try
            {
                if (_SerialPort != null && _SerialPort.IsOpen)
                {
                    _SerialPort.Dispose();
                    _SerialPort.Close();
                }

                _SerialPort.PortName = SelectedCommPort.DeviceID;
                _SerialPort.BaudRate = SelectedBaudRate;
                _SerialPort.Parity = SelectedParity;
                _SerialPort.DataBits = SelectedDataBits;
                _SerialPort.StopBits = SelectedStopBits;
                _SerialPort.Open();
                _SerialPort.DtrEnable = IsDTR;
                _SerialPort.RtsEnable = IsRTS;

                OutputText = "";
                OnPropertyChanged("OutputText");

                EnableDisableSettings = false;
                OnPropertyChanged("EnableDisableSettings");


                _SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEvent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            OnPropertyChanged("WindowTitle");
        }

        /// <summary>
        /// Allow/disallow StartListening() to be executed.
        /// </summary>
        /// <returns>True/False</returns>
        private bool StartListeningCanExecute()
        {
            if (CommPorts == null)
            {
                return false;
            }
            else
            {
                if (_SerialPort == null || !_SerialPort.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Terminate serial port communication.
        /// </summary>
        private void StopListening()
        {
            if (_SerialPort != null && _SerialPort.IsOpen)
            {
                try
                {
                    _SerialPort.DataReceived -= DataReceivedEvent;
                    _SerialPort.Dispose();
                    _SerialPort.Close();

                    EnableDisableSettings = true;
                    OnPropertyChanged("EnableDisableSettings");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            OnPropertyChanged("WindowTitle");
        }

        /// <summary>
        /// Get connection/communication status.
        /// </summary>
        /// <returns>String of Connected/Disconnect</returns>
        private string GetConnectionStatus()
        {
            if (_SerialPort != null && _SerialPort.IsOpen)
                return "Connected";
            else
                return "Not Connected";
        }

        /// <summary>
        /// Rescan avaiable ports
        /// </summary>
        private void RefreshPortsMethod()
        {
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.getCommPorts();
                OnPropertyChanged("CommPorts");
                SelectedCommPort = CommPorts[0];
                OnPropertyChanged("SelectedCommPort");
            }
            catch (Exception)
            {
                //
            }
        }

        private void ChangeFileLocationMethod()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "File Location";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = FileLocation;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = FileLocation;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                // Do something with selected folder string
                FileLocation = folder.ToString();
                OnPropertyChanged("FileLocation");
            }
        }

        private void ExportFileMethod()
        {
            try
            {
                if (File.Exists(FileLocation + @"\" + FileName + SelectedFileExtension))
                {
                    MessageBoxResult msgBoxResult = MessageBox.Show(
                        "File " + FileName + SelectedFileExtension + " already exists!\n Select 'Yes' to overwrite the existing file or\n'No' to create a new file with timestamp suffix or\n 'Cancel' to cancel?",
                        "Overwrite Confirmation",
                        MessageBoxButton.YesNoCancel);
                    if (msgBoxResult == MessageBoxResult.Yes)
                    {
                        File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, OutputText);
                        ExportStatus = "Done.";
                        OnPropertyChanged("ExportStatus");
                        ExportStatusSuccess = true;
                        OnPropertyChanged("ExportStatusSuccess");
                        StartTimer(10);

                    }
                    else if (msgBoxResult == MessageBoxResult.No)
                    {
                        File.WriteAllText(FileLocation + @"\" + FileName + DateTime.Now.ToString("-yyyyMMddHHmmss") + SelectedFileExtension, OutputText);
                        ExportStatus = "Done.";
                        OnPropertyChanged("ExportStatus");
                        ExportStatusSuccess = true;
                        OnPropertyChanged("ExportStatusSuccess");
                        StartTimer(10);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, OutputText);
                    ExportStatus = "Done.";
                    OnPropertyChanged("ExportStatus");
                    ExportStatusSuccess = true;
                    OnPropertyChanged("ExportStatusSuccess");
                    StartTimer(10);
                }
            }
            catch (Exception ex)
            {
                ExportStatus = "Error exporting a file!";
                OnPropertyChanged("ExportStatus");
                ExportStatusSuccess = false;
                OnPropertyChanged("ExportStatusSuccess");
                StartTimer(10);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ExportFileCanExecute()
        {
            return FileName != "";
        }

        private void StartTimer(int duration)
        {
            if (timer != null)
            {
                timer.Stop();
                ExportStatus = "";
                OnPropertyChanged("ExportStatus");
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }
        #endregion
    }
}
