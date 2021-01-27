using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AmongServers.Launcher.Utilities
{
    /// <summary>
    /// Represents a region server.
    /// </summary>
    public class RegionServer
    {
        /// <summary>
        /// The name of the region.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The endpoint.
        /// </summary>
        public IPEndPoint Endpoint { get; set; }
    }
}
