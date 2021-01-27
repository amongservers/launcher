using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// Represents a game (lobby) entity.
    /// </summary>
    public class GameEntity
    {
        /// <summary>
        /// The game state.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// The game map.
        /// </summary>
        [JsonProperty("map")]
        public string Map { get; set; }

        /// <summary>
        /// The maximum number of players.
        /// </summary>
        [JsonProperty("maxPlayers")]
        public int MaxPlayers { get; set; }

        /// <summary>
        /// The number of players currently playing/waiting.
        /// </summary>
        [JsonProperty("countPlayers")]
        public int CountPlayers { get; set; }

        /// <summary>
        /// The number of imposters the game will/currently have.
        /// </summary>
        [JsonProperty("numImposters")]
        public int NumImposters { get; set; }

        /// <summary>
        /// If the game is public.
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// The game host, this will be null if the game is private.
        /// </summary>
        [JsonProperty("hostPlayer")]
        public PlayerEntity HostPlayer { get; set; }

        /// <summary>
        /// The players, this array will be empty if the game is private.
        /// </summary>
        [JsonProperty("players")]
        public PlayerEntity[] Players { get; set; }
    }
}
