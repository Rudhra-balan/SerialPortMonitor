using System;
using PortMoniter.Models;
using VirtualPortBus;

namespace PortMoniter
{
    /// <summary>
    /// Represents the library global settings.
    /// </summary>
    public class Global
    {
        private static readonly Lazy<Global> LazyDefault = new Lazy<Global>(() => new Global());

        /// <summary>
        /// Gets the default global settings instance.
        /// </summary>
        public static Global Default => LazyDefault.Value;

        /// <summary>
        /// Prevents a default instance of the <see cref="Global"/> class from being created.
        /// </summary>
        private Global()
        {

        }

        public PortInfo PortInfo { get; set; } = new PortInfo();
        
        public Com0ComSetupCFacade Com0ComFacade { get; } = new Com0ComSetupCFacade();
    }
}
