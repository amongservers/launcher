using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// Represents the server entity.
    /// </summary>
    public class ServerEntity
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        [JsonProperty("ipAddress")]
        public string IPAddress { get; set; }

        /// <summary>
        /// The port of the server.
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }
    }
}
