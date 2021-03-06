﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// Represents the server entity.
    /// </summary>
    public class ServerEntity : HeartbeatEntity
    {
        /// <summary>
        /// The time the server last send a heartbeat.
        /// </summary>
        [JsonProperty("lastSeenAt")]
        public DateTimeOffset LastSeenAt { get; set; }
    }
}
