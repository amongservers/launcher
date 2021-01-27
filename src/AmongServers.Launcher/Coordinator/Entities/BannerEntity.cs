using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongServers.Launcher.Coordinator.Entities
{
    /// <summary>
    /// A banner, used for updates and website promo.
    /// </summary>
    public class BannerEntity
    {
        /// <summary>
        /// The image URL.
        /// </summary>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The alt text for accessibility.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The link.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}
