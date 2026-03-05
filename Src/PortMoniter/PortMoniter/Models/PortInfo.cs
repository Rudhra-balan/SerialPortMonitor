using System.IO.Ports;

namespace PortMoniter.Models
{
    public class PortInfo
    {

        public string SimulatedPortName = "COM1";
        public string RealPortName = "COM2";
        public string Forwarding = "COM3";
        public int BaudRate = 9600;
        public int DataBits = 8;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        public Handshake Handshake = Handshake.None;
    }
}
