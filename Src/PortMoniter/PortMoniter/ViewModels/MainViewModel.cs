using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.AppCenter.Crashes;
using Microsoft.WindowsAPICodePack.Dialogs;
using PortMoniter.Controls;
using PortMoniter.Models;
using PortMoniter.Wrapper;
using SerialSniffer;
using VirtualPortBus.Models;

namespace PortMoniter.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        #region Private Variable

        private static readonly string AppTitle = nameof(SerialPort)+" Monitor ";
        private static bool applicationLoaded = false;
        private bool _IsAutoscrollChecked = true;

        #endregion

        #region ObservableProperty

        public string ScrollConfirm
        {
            get
            {
                return "Autoscroll";
            }
        }
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
        private ObservableCollection<string> _availablePorts;

        public ObservableCollection<string> AvailablePorts
        {
            get => _availablePorts;
            set
            {
                _availablePorts = value;
                OnPropertyChanged("AvailablePorts");
            }
        }

        private string _realPort;
        public string RealPort
        {
            get => _realPort;
            set
            {
                _realPort = value;
                if (_realPort != null)
                {
                    Global.Default.PortInfo.RealPortName = value;
                }
                OnPropertyChanged("RealPort");
            }
        }
        private ObservableCollection<CrossoverPortPair> _virtualPorts;
        public ObservableCollection<CrossoverPortPair> VirtualPorts
        {
            get => _virtualPorts;
            set
            {

                _virtualPorts = value;
                OnPropertyChanged("VirtualPorts");
            }
        }

        private CrossoverPortPair _virtualPort;
        public CrossoverPortPair VirtualPort
        {
            get => _virtualPort;
            set
            {
               
                _virtualPort = value;
                if (_virtualPort != null)
                {
                    Global.Default.PortInfo.SimulatedPortName = _virtualPort.PortNameB;
                }

                OnPropertyChanged("VirtualPort");
            }
        }
        private ObservableCollection<int> _baudRate;

        public ObservableCollection<int> BaudRate
        {
            get => _baudRate;
            set
            {
                _baudRate = value;
                OnPropertyChanged("BaudRate");
            }
        }

        private int _selectedBaudRate;

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set
            {
                Global.Default.PortInfo.BaudRate = value;
                _selectedBaudRate = value;
                OnPropertyChanged("SelectedBaudRate");
            }
        }

        private ObservableCollection<int> _dataBits;

        public ObservableCollection<int> DataBits
        {
            get => _dataBits;
            set
            {
                _dataBits = value;
                OnPropertyChanged("DataBits");
            }
        }

        private int _selectedDataBits;

        public int SelectedDataBits
        {
            get => _selectedDataBits;
            set
            {
                Global.Default.PortInfo.DataBits = value;
                
                _selectedDataBits = value;
                OnPropertyChanged("SelectedDataBits");
            }
        }


        private ObservableCollection<string> _parity;

        public ObservableCollection<string> Parity
        {
            get => _parity;
            set
            {
                _parity = value;
                OnPropertyChanged("Parity");
            }
        }

        private string _selectedParity;
        public string SelectedParity
        {
            get => _selectedParity;
            set
            {
                 Enum.TryParse<Parity>(value, out var tempBaudRate);
                Global.Default.PortInfo.Parity = tempBaudRate;
                _selectedParity = value;
                OnPropertyChanged("SelectedParity");
            }
        }


        private ObservableCollection<string> _stopBits;

        public ObservableCollection<string> StopBits
        {
            get => _stopBits;
            set
            {
                _stopBits = value;
                OnPropertyChanged("StopBits");
            }
        }

        private string _selectedStopBits;
        public string SelectedStopBits
        {
            get => _selectedStopBits;
            set
            {
                Enum.TryParse<StopBits>(value, out var tempStopBits);
                Global.Default.PortInfo.StopBits = tempStopBits;
                _selectedStopBits = value;
                OnPropertyChanged("SelectedStopBits");
            }
        }
        public bool ScrollOnTextChanged { get; private set; }

        private ObservableCollection<string> _outputFormat;

        public ObservableCollection<string> OutputFormat
        {
            get => _outputFormat;
            set
            {
                _outputFormat = value;
                OnPropertyChanged("OutputFormat");
            }
        }

        private string _selectedOutputFormat;

        public string SelectedOutputFormat
        {
            get => _selectedOutputFormat;
            set
            {
                Enum.TryParse(value, out ByteEnumerableExtensions.Format formate);
                GlobalParameters.IsOnlyAscii = formate == ByteEnumerableExtensions.Format.OnlyAscii;
                GlobalParameters.IsOnlyHex = formate == ByteEnumerableExtensions.Format.OnlyHex;
                _selectedOutputFormat = value;
                OnPropertyChanged("SelectedOutputFormat");
            }
        }

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

        public bool IsShowTime
        {
            get => GlobalParameters.IsShowTime;
            set
            {
                GlobalParameters.IsShowTime = value;
                OnPropertyChanged("IsShowTime");
            }
        }


        #endregion

        #region Public ICommands


        public ICommand CreateVirtualPort
        {
            get
            {
                _startMonitoring = new ConditionalRelayCommand(param => CreateVirtualPortPair(),
                    param => !VirtualPorts.Any() || VirtualPorts == null);
                return _startMonitoring;
            }

        }
        public ICommand DeleteVirtualPort
        {
            get
            {
                _startMonitoring = new ConditionalRelayCommand(param => DeleteVirtualPortPair(),
                    param => VirtualPorts.Any());
                return _startMonitoring;
            }
        }


        private ICommand _startMonitoring;

        public ICommand StartMonitoring
        {
            get
            {
                _startMonitoring = new ConditionalRelayCommand(param => StartListening(),
                    param => StartListeningCanExecute());
                return _startMonitoring;
            }
        }

        private ICommand _clear;
        public ICommand Clear
        {
            get
            {
                _clear = new ConditionalRelayCommand(
                    param => ClearFileMethod(),
                    param => ClearFileCanExecute()); 
                return _clear;
            }
        }

       
        private ICommand _exportTxtFile;
       

        private ICommand _close;
        public ICommand Close
        {
            get
            {
                _close = new ConditionalRelayCommand(
                    param => StopListening(),
                    param => CommPortSniffer.Instance.IsOpen);
                return _close;
            }
        }

        private ICommand _refreshPorts;
        public ICommand RefreshPorts
        {
            get
            {
                _refreshPorts = new ConditionalRelayCommand(
                    param => LoadAvailablePort(),
                    param =>  !CommPortSniffer.Instance.IsOpen);
                return _refreshPorts;
            }
        }

        #endregion

        #region Public Variable

        private static readonly StringBuilder FormatedString = new StringBuilder();

        public string OutputText { get; private set; }
        public bool EnableDisableSettings { get; private set; }

        public string WindowTitle { get; private set; }

        public string SelectedLineEnding { get; private set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            GlobalParameters.BytesPerLine = 16;
            IsShowTime = true;
            LoadAvailablePort();

            #region BaudRate

            BaudRate = new ObservableCollection<int>(new List<int>
            {
                100,300,600,1200,2400,4800,9600,14400,19200,
                38400,56000,57600,115200,128000,256000,0
            });
            SelectedBaudRate = 9600;

            #endregion

            #region DataBits


            DataBits = new ObservableCollection<int>(new List<int>()
            {
                5,6,7,8
            });

            SelectedDataBits = 8;

            #endregion

            #region Parity

            Parity = new ObservableCollection<string>();
            foreach (string item in Enum.GetNames(typeof(Parity)))
            {
                Parity.Add(item);
            }
            SelectedParity = System.IO.Ports.Parity.None.ToString();

            #endregion

            #region StopBits

            StopBits = new ObservableCollection<string>();
            foreach (string item in Enum.GetNames(typeof(StopBits)))
            {
                StopBits.Add(item);
            }
            SelectedStopBits = System.IO.Ports.StopBits.One.ToString();

            #endregion

            #region Virtual Ports
            VirtualPorts = new ObservableCollection<CrossoverPortPair>();
            var virtualPort = GetVirtualPorts();
            if (virtualPort.Count > 0)
            {
                foreach (var portPair in virtualPort)
                {
                    VirtualPorts.Add(portPair);
                }
                
                VirtualPort = VirtualPorts[0];
            }

            ScrollOnTextChanged = true;


            #endregion

            #region Output Format

            OutputFormat = new ObservableCollection<string>();
            foreach (var formatName in Enum.GetNames(typeof(ByteEnumerableExtensions.Format)))
            {
                OutputFormat.Add(formatName);
            }

            SelectedOutputFormat = ByteEnumerableExtensions.Format.Plain.ToString();

            #endregion
            SelectedLineEnding = "";
            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";

            StartButtonVisible = Visibility.Visible;
            StopButtonVisible = Visibility.Hidden;

            applicationLoaded = true;

            FileLocation = AssemblyDirectory;
            FileName = "output_data";
            SelectedFileExtension = ".TXT";
            FileExtensions = new[] { ".TXT", ".CSV" };
        }

        ~MainViewModel()
        {

        }
        #endregion

        #region Virtual Port

        private void CreateVirtualPortPair()
        {
            try
            {
                List<string> ports = new List<string>();

                if (CommPortSniffer.Instance.IsOpen) return;

                var availableSerialPorts = SerialPortSettingsModel.Instance.getCommPorts();

                if (availableSerialPorts.Any())
                {
                    ports = availableSerialPorts.Select(port => port.DeviceID).ToList();
                }

                var availablePort = ports.OrderByDescending(x => x).ToList();

                if (availablePort.Any())
                {
                    var lastIndex = ExtractInt(availablePort[0]);
                    Global.Default.Com0ComFacade.CreatePortPair("COM" + (lastIndex + 1), "COM" + (lastIndex + 2));
                }

                var virtualPort = Global.Default.Com0ComFacade.GetCrossoverPortPairs().ToList();

                VirtualPorts = new ObservableCollection<CrossoverPortPair>();
                foreach (var portPair in virtualPort)
                {
                    VirtualPorts.Add(portPair);
                }

                if (VirtualPorts.Any())
                {
                    VirtualPort = VirtualPorts[0];
                }

                MessageBox.Show("Success fully created the virtual port");

            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error :" + e.Message);
                Crashes.TrackError(e);
            }

        }

        private void DeleteVirtualPortPair()
        {
            try
            {
                if (CommPortSniffer.Instance.IsOpen) return;
                if (VirtualPort == null)
                {
                    MessageBox.Show("Please select the port to delete");
                    return;
                }
                Global.Default.Com0ComFacade.DeletePortPair(VirtualPort.PairNumber);
                var virtualPort = Global.Default.Com0ComFacade.GetCrossoverPortPairs().ToList();
                VirtualPorts = new ObservableCollection<CrossoverPortPair>();
                foreach (var portPair in virtualPort)
                {
                    VirtualPorts.Add(portPair);
                }

                if (VirtualPorts.Any())
                {
                    VirtualPort = VirtualPorts[0];
                }
                MessageBox.Show("Success fully Deleted the virtual port");
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error :" + e.Message);
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Start Monitoring


        /// <summary>
        /// Initiate serial port communication.
        /// </summary>
        private void StartListening()
        {
            try
            {

                CommPortSniffer com = CommPortSniffer.Instance;

                com.StatusChanged += OnStatusChanged;
                com.DataReceived += OnDataReceived;
                var isOpen = com.Open();
                if (isOpen)
                {
                    OutputText = "";
                    OnPropertyChanged("OutputText");
                    EnableDisableSettings = false;
                    OnPropertyChanged("EnableDisableSettings");
                    StopButtonVisible = Visibility.Visible;
                    StartButtonVisible = Visibility.Hidden;
                }

                WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
                OnPropertyChanged("WindowTitle");
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Crashes.TrackError(e);
            }
           
        }

        /// <summary>
        /// Allow/disallow StartListening() to be executed.
        /// </summary>
        /// <returns>True/False</returns>
        private bool StartListeningCanExecute()
        {
            return !CommPortSniffer.Instance.IsOpen;
        }


        /// <summary>
        /// Handle data received event from serial port.
        /// </summary>
        /// <param name="dataIn">incoming data</param>
        public void OnDataReceived(string dataIn)
        {
            FormatedString.AppendLine(dataIn);
            OutputText = FormatedString.ToString();
            OnPropertyChanged("OutputText");
        }

        /// <summary>
        /// Update the connection status
        /// </summary>
        public void OnStatusChanged(string status)
        {
            OutputText = status;
            OnPropertyChanged("OutputText");
        }

        #endregion

        #region Stop Monitoring

        /// <summary>
        /// Terminate serial port communication.
        /// </summary>
        private void StopListening()
        {
            CommPortSniffer com = CommPortSniffer.Instance;
            if (com.IsOpen)
            {
                try
                {
                    com.StatusChanged -= OnStatusChanged;
                    com.DataReceived -= OnDataReceived;
                    com.Close();
                    EnableDisableSettings = true;
                    OnPropertyChanged("EnableDisableSettings");
                    StopButtonVisible = Visibility.Hidden;
                    StartButtonVisible = Visibility.Visible;

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Crashes.TrackError(ex);
                }
            }

            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            OnPropertyChanged("WindowTitle");
        }

        #endregion

        #region Export File


        private DispatcherTimer timer = null;
        private string _fileName = "output_data";
       
        public string FileLocation { get; set; }
        private ICommand _ChangeFileLocation;

        private ICommand _ExportTXTFile;
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
        public string SelectedFileExtension { get; set; }
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
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

        public string ExportStatus { get; set; }
        public bool ExportStatusSuccess { get; set; }

      
        private void ExportFileMethod()
        {
            try
            {
                if (File.Exists(FileLocation + @"\" + FileName + SelectedFileExtension))
                {
                    MessageBoxResult msgBoxResult = MessageBox.Show(
                        "File " + FileName + SelectedFileExtension +
                        " already exists!\n Select 'Yes' to overwrite the existing file or\n'No' to create a new file with timestamp suffix or\n 'Cancel' to cancel?",
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
                        File.WriteAllText(
                            FileLocation + @"\" + FileName + DateTime.Now.ToString("-yyyyMMddHHmmss") +
                            SelectedFileExtension, OutputText);
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
            catch (System.Exception ex)
            {
                ExportStatus = "Error exporting a file!";
                OnPropertyChanged("ExportStatus");
                ExportStatusSuccess = false;
                OnPropertyChanged("ExportStatusSuccess");
                StartTimer(10);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Crashes.TrackError(ex);
            }


        }

        private bool ExportFileCanExecute()
        {
            return !string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(OutputText);
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


        private void StartTimer(int duration)
        {
            if (timer != null)
            {
                timer.Stop();
                ExportStatus = "";
                OnPropertyChanged("ExportStatus");
            }
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(duration)
            };
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
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

        #region Clear

        private void ClearFileMethod()
        {
           OutputText = string.Empty;
           OnPropertyChanged("OutputText");
        }

        private bool ClearFileCanExecute()
        {
            return !string.IsNullOrEmpty(OutputText);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Get connection/communication status.
        /// </summary>
        /// <returns>String of Connected/Disconnect</returns>
        private string GetConnectionStatus()
        {
            return CommPortSniffer.Instance.IsOpen ? "Connected" : "Not Connected";
        }

        private List<CrossoverPortPair> GetVirtualPorts()
        {
            var virtualPort = Global.Default.Com0ComFacade.GetCrossoverPortPairs().ToList();
            if (!virtualPort.Any())
            {
                var availablePort = SerialPort.GetPortNames().OrderByDescending(x => x).ToList();

                if (availablePort.Any())
                {
                    var lastIndex = ExtractInt(availablePort[0]);
                    Global.Default.Com0ComFacade.CreatePortPair("COM" + (lastIndex + 1), "COM" + (lastIndex + 2));
                }
                virtualPort = Global.Default.Com0ComFacade.GetCrossoverPortPairs().ToList();
            }

            if (virtualPort.Count <= 1) return virtualPort.ToList();
            foreach (var crossoverPortPair in virtualPort)
            {
                Global.Default.Com0ComFacade.DeletePortPair(crossoverPortPair.PairNumber);
            }
            virtualPort = Global.Default.Com0ComFacade.GetCrossoverPortPairs().ToList();

            return virtualPort.ToList();


        }

        private int ExtractInt(string input)
        {
            string temp = string.Empty;
            int val = 500;

            temp = input.Where(char.IsDigit).Aggregate(temp, (current, t) => current + t);

            if (temp.Length > 0)
                val = int.Parse(temp);

            return val;
        }

        #endregion

        #region Initalize Method


        public void LoadAvailablePort()
        {
            EnableDisableSettings = true;
            OnPropertyChanged("EnableDisableSettings");
            var virtualPort = GetVirtualPorts();
            string[] serialPort = SerialPort.GetPortNames();
            var portList = serialPort.ToList();
            foreach (var portPair in virtualPort)
            {

                if (serialPort.Contains(portPair.PortNameA))
                {
                    portList.Remove(portPair.PortNameA);
                }

                if (serialPort.Contains(portPair.PortNameB))
                {
                    portList.Remove(portPair.PortNameB);
                }
            }

            AvailablePorts = new ObservableCollection<string>();
            foreach (var name in portList)
            {
                AvailablePorts.Add(name);
            }
            if (AvailablePorts.Count > 0)
                RealPort = AvailablePorts[0];

            VirtualPorts = new ObservableCollection<CrossoverPortPair>();
            foreach (var portPair in virtualPort)
            {
                VirtualPorts.Add(portPair);
            }

            if(VirtualPorts.Count>0)
                VirtualPort = VirtualPorts[0];

        }
        #endregion

    }
}