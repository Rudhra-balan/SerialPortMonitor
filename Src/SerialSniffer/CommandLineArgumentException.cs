
using System;

namespace SerialSniffer
{
    /// <summary>
    /// An expection thrown when a command line argument is missing or some 
    /// parsing error occurs.
    /// </summary>
    public class CommandLineArgumentException : Exception
    {
        public CommandLineArgumentException()
            : base("CommandLineArgumentException: Unknown")
        {
        }

        public CommandLineArgumentException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
