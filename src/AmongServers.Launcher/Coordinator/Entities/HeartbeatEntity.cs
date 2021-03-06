﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// Represents a heartbeat request entity.
    /// </summary>
    public class HeartbeatEntity
    {
        /// <summary>
        /// The server endpoint.
        /// </summary>
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        /// <summary>
        /// The server name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The running games, only includes pending, starting and running lobbies.
        /// </summary>
        [JsonProperty("games")]
        public GameEntity[] Games { get; set; }
    }
}
