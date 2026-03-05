using SerialSniffer;
using System;

namespace PortMoniter.Wrapper
{
    public sealed class CommPortSniffer
    {
        private Sniffer sniffer;

        //begin Singleton pattern
        static readonly CommPortSniffer instance = new CommPortSniffer();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CommPortSniffer()
        {
        }

        public static CommPortSniffer Instance => instance;
        //end Singleton pattern

        //begin Observer pattern
        public delegate void EventHandler(string param);
        public EventHandler StatusChanged;
        public EventHandler DataReceived;
        //end Observer pattern


        /// <summary> Open the serial port with current settings. </summary>
        public bool Open()
        {
            DateTime start = DateTime.MinValue;
            bool isFirst = true;

            try
            {
                sniffer = new Sniffer(Global.Default.PortInfo.SimulatedPortName, Global.Default.PortInfo.RealPortName,
                    Global.Default.PortInfo.BaudRate, Global.Default.PortInfo.Parity, Global.Default.PortInfo.StopBits, Global.Default.PortInfo.DataBits)
                    {
                        Mode = SnifferMode.Simulate,
                        IsCollapsingSameOrigin = false
                    };


                sniffer.PacketAvailable += (s, e) =>
                {
                    if (isFirst)
                    {
                        start = e.When;
                        isFirst = false;
                    }
                    string arrivedPacket = DecodeArrivedPacket(e, start);
                    DataReceived(arrivedPacket);
                };
                sniffer.OpenAndSniff();
                return true;
            }
            
            catch (Exception ex)
            {
                StatusChanged($"{ex.ToString()}");
            }
            return false;
        }

        private static object objLock = new object();
        /// <summary>
        /// Computes a string that shows a decoded version of the sniffed packet.
        /// </summary>
        /// <param name="e">Argument object containing the packed to be decoded.</param>
        /// <param name="start">Start time when the packet has been sniffed. This time is reliable when the <see cref="GlobalParameters.IsShowCollapsed"/> is false. When
        /// it is true, it should be considered that the packet could be composed of strings of characters loaded non contiguously.</param>
        /// <returns>>The decoded version of the sniffed packet.</returns>
        public static string DecodeArrivedPacket(SniffedPacketEventArgs e, DateTime start)
        {
            lock (objLock)
            {
                string preamble;
                if (GlobalParameters.IsShowTime)
                {
                    preamble = string.Format(
                        "{0:yyyy-MM-dd HH mm ss.fff} {1} ",
                        e.When,
                        e.Origin == Origin.FromReal ? "[RD]" : "[WR]");
                }
                else
                {
                    preamble = string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0,10:0.000} {1} ",
                        e.When.Subtract(start).TotalMilliseconds,
                        e.Origin == Origin.FromReal ? "[RD]" : "[WR]");
                }

                return e.Content.ToHex(preamble, GlobalParameters.OutputFormat, GlobalParameters.BytesPerLine);
            }
        }

        /// <summary> Close the serial port. </summary>
        public void Close()
        {
            if (sniffer != null && sniffer.IsOpen)
            {
                sniffer.CloseSniff();
            }

            if (StatusChanged != null)
                StatusChanged("connection closed");
        }

        /// <summary> Get the status of the serial port. </summary>
        public bool IsOpen => sniffer != null && sniffer.IsOpen;


    }
}
