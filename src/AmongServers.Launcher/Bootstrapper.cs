using AmongServers.Launcher.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmongServers.Launcher
{
    /// <summary>
    /// Provides functions to alter region data and launch the game.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Replaces the region info file.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <returns></returns>
        public static Task ReplaceRegionInfoAsync(string serverName, IPEndPoint serverEndpoint)
        {
            string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow");
            fullPath = Path.Combine(fullPath, "Innersloth", "Among Us", "regionInfo.dat");

            RegionInfo regionInfo = new RegionInfo();
            regionInfo.PingEndpoint = serverEndpoint;
            regionInfo.Name = serverName;
            regionInfo.Servers.Add(new RegionServer() {
                Name = serverName,
                Endpoint = serverEndpoint
            });

            return regionInfo.SaveAsync(fullPath)
                .AsTask();
        }

        /// <summary>
        /// Launches the game.
        /// </summary>
        /// <returns></returns>
        public static Task LaunchGameAsync()
        {
            return Task.Run(() => {
                Process.Start(new ProcessStartInfo("steam://rungameid/945360") {
                    UseShellExecute = true
                });
            });
        }
    }
}
