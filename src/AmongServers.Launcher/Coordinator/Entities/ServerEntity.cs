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

        /// <summary>
        /// The number of active players.
        /// </summary>
        [JsonProperty("countPlayers")]
        public int CountPlayers { get; set; }

        /// <summary>
        /// The number of pending lobbies.
        /// </summary>
        [JsonProperty("countPendingLobbies")]
        public int CountPendingLobbies { get; set; }

        /// <summary>
        /// The number of active lobbies.
        /// </summary>
        [JsonProperty("countActiveLobbies")]
        public int CountActiveLobbies { get; set; }

        /// <summary>
        /// The time the server last send a heartbeat.
        /// </summary>
        [JsonProperty("lastSeenAt")]
        public DateTimeOffset LastSeenAt { get; set; }
    }
}
