using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Utilities
{
    /// <summary>
    /// Represents a favourited server.
    /// </summary>
    public class FavouriteServer
    {
        /// <summary>
        /// The endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The last known name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time the server was added.
        /// </summary>
        public DateTimeOffset AddedAt { get; set; }
    }
}
