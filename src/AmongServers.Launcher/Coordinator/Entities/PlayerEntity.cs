using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public class PlayerEntity
    {
        /// <summary>
        /// The player name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
